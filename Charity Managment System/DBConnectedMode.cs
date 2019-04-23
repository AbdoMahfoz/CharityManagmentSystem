﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CharityManagmentSystem.Models;
using System.Data;

namespace CharityManagmentSystem
{
    class DBConnectedMode : IDBLayer
    {
        OracleConnection conn;
        public void InitializeConnection()
        {
            conn = new OracleConnection(DBGlobals.ConnectionString);
            conn.Open();
        }
        public void TerminateConnection()
        {
            conn.Close();
        }
        private T FillObject<T>(OracleDataReader reader) where T : new()
        {
            string[] list = new string[reader.FieldCount];
            for(int i = 0; i < reader.FieldCount; i++)
            {
                list[i] = reader.GetName(i);
            }
            T res = new T();
            foreach(var Property in typeof(T).GetFields())
            {
                string x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == Property.Name));
                if(!string.IsNullOrEmpty(x))
                {
                    Property.SetValue(res, reader[x]);
                }
            }
            return res;
        }
        private T[] FillList<T>(string cmdstr, params KeyValuePair<string, object>[] args) where T : new()
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = cmdstr,
                CommandType = CommandType.Text
            };
            foreach(var arg in args)
            {
                cmd.Parameters.Add(arg.Key, arg.Value);
            }
            OracleDataReader reader = cmd.ExecuteReader();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                T c = FillObject<T>(reader);
                list.Add(c);
            }
            reader.Close();
            return list.ToArray();
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            return FillList<Beneficiary>("select * from Beneficiary b, Person p where b.SSN = p.SSN");

        }
        public Campaign[] GetAllCampaigns()
        {
            return FillList<Campaign>("select * from Campaign");

        }
        public Category[] GetAllCategories()
        {
            return FillList<Category>("select * from Category");

        }
        public Department[] GetAllDepartments()
        {
            return FillList<Department>("select * from Department");

        }
        public Donor[] GetAllDonors()
        {
            return FillList<Donor>("select * from Donor d, Person p where d.SSN = p.SSN");

        }
        public Employee[] GetAllEmployees()
        {
            return FillList<Employee>("select * from Employee e, Person p where e.SSN = p.SSN");
        }
        public Item[] GetAllItems()
        {
            //return FillList<Item>("select * from Item i, MainCategory MC,SubCategory SC "+
            //  "where i.Name = MC.Name and i.Name = SC.Name");
            throw new NotImplementedException();
        }
        public MainCategory[] GetAllMainCategories()
        {
            return FillList<MainCategory>("select * from MainCategory MC ,Category C where MC.Name = C.Name");
        }
        public Person[] GetAllPersons()
        {
            return FillList<Person>("select * from Person");
        }
        public Recepient[] GetAllRecepients()
        {
            return FillList<Recepient>("select * from Recipient r,Person p where r.recipient_ssn=p.ssn");
        }
        public SubCategory[] GetAllSubCategories()
        {
            return FillList<SubCategory>("select * from SubCategory sub,category c where sub.name=c.name");
        }
        public Volunteer[] GetAllVolunteers()
        {
            return FillList<Volunteer>("select * from Volunteer");
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            return FillList<Beneficiary>(@"select beneficiary_ssn from Beneficiary b ,Benefit_from bf , Campaign C 
                                         where b.beneficiary_ssn=bf.beneficiary_ssn and bf.campaign_id=C.campaign_id");
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
        {
            return FillList<Department>("select dept_name,description from department dep , Employee emp where dep.dept_name=emp.department_name");
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            return FillList<Donor>(@"select * from Donate_to DT, Donor D, Person P 
                                    where DT.Campaign_ID = :IDT and DT.Donor_SSN = D.Donor_SSN and D.Donor_SSN = P.SSN",
                            new KeyValuePair<string, object>("IDT", campaign.ID));
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = @"select * from Donate_to DT , Donor D, Person P
                              where DT.Campaign_ID = :IDT and DT.Donor_SSN = D.Donor_SSN and D.Donor_SSN = P.SSN and
                              DT.ItemName = :IName and DT.ItemMainName = :IMN and DT.ItemSubName = :ISN ",
                CommandType = CommandType.Text
            };
            cmd.Parameters.Add("IDT", campaign.ID);
            cmd.Parameters.Add("IName", item.Name);
            cmd.Parameters.Add("IMN", item.Main);
            cmd.Parameters.Add("ISN", item.Sub);
            OracleDataReader reader = cmd.ExecuteReader();
            List<DonorItem> list = new List<DonorItem>();
            while (reader.Read())
            {
                list.Add(new DonorItem()
                {
                    Donor = FillObject<Donor>(reader),
                    Campaign = campaign,
                    Item = item,
                    Count = (int)reader["Count_"]
                });
            }
            reader.Close();
            return list.ToArray();
        }
        public Employee GetEmployeeManaging(Campaign campaign)
        {
            return FillList<Employee>(@"select * from Campaign C, Employee E, Person P 
                                where C.ID_ = :IDT and C.Employee_SSN = E.Employee_SSN and E.Employee_SSN = P.SSN",
                                new KeyValuePair<string, object>("IDT", campaign.ID))[0];
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            return FillList<Employee>(@"select * from Employee E , Person P 
                                        where E.Department_Name = :DN and E.Employee_SSN = P.SSN",
                            new KeyValuePair<string, object>("DN", department.DeptName));
        }
        public Item[] GetItemsDonatedBy(Donor donor)
        {
            return FillList<Item>("select * from Donate_to DT where DT.Donor_SSN = :DN",
                            new KeyValuePair<string, object>("DN", donor.SSN));
        }
        public Item[] GetItemsIn(Campaign campaign)
        {
            return FillList<Item>("select * from Donate_to RF where RF.Campaign_ID = :IDT",
                            new KeyValuePair<string, object>("IDT", campaign.ID));
        }
        public Item[] GetItemsOf(MainCategory mainCategory)
        {
            return FillList<Item>("select * from Item I where I.ItemMainName = :IMN",
                            new KeyValuePair<string, object>("IMN", mainCategory.Name));
        }
        public Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory)
        {
            return FillList<Item>("select * from Item I where I.ItemMainName = :IMN and I.ItemSubName = :ISN",
                            new KeyValuePair<string, object>("IMN", mainCategory.Name),
                            new KeyValuePair<string, object>("ISN", subCategory.Name));
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
        {
            return FillList<Item>("select * from Receives_From RF where RF.Recipient_SSN = :RSSN",
                            new KeyValuePair<string, object>("RSSN", recepient.SSN));
        }
        public RecepientItem[] GetRecepientsOf(Campaign campaign, Item item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = @"select * from Receives_From RF , Recipient R, Person P
                              where RF.Campaign_ID = :IDT and RF.Donor_SSN = R.Donor_SSN and R.Donor_SSN = P.SSN and
                              DT.ItemName = :IName and DT.ItemMainName = :IMN and DT.ItemSubName = :ISN ",
                CommandType = CommandType.Text
            };
            cmd.Parameters.Add("IDT", campaign.ID);
            cmd.Parameters.Add("IName", item.Name);
            cmd.Parameters.Add("IMN", item.Main);
            cmd.Parameters.Add("ISN", item.Sub);
            OracleDataReader reader = cmd.ExecuteReader();
            List<RecepientItem> list = new List<RecepientItem>();
            while (reader.Read())
            {
                list.Add(new RecepientItem()
                {
                    Recipient = FillObject<Recepient>(reader),
                    Campaign = campaign,
                    Item = item,
                    Count = (int)reader["Count_"]
                });
            }
            reader.Close();
            return list.ToArray();
        }
        //
        public Recepient[] GetRecepientsReceivingFrom(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Volunteer[] GetVolunteersOf(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Department GetDepartmentOf(Employee employee)
        {
            throw new NotImplementedException();
        }
        public Campaign[] GetCampaginsManagedBy(Employee employee)
        {
            throw new NotImplementedException();
        }
        public Campaign[] GetCampaignsOf(Volunteer volunteer)
        {
            throw new NotImplementedException();
        }
        public Campaign[] GetCampaignsOf(Donor donor)
        {
            throw new NotImplementedException();
        }
        public Campaign[] GetCampaignsOf(Recepient recepient)
        {
            throw new NotImplementedException();
        }
        
        public Campaign[] GetCampaignsOf(Beneficiary beneficiary)
        {
            throw new NotImplementedException();
        }
        public DonorItem[] GetDonorsOf(Item item)
        {
            throw new NotImplementedException();
        }
        public SubCategory[] GetSubCategoriesOf(MainCategory mainCategory)
        {
            throw new NotImplementedException();
        }
        public void InsertPersons(params Person[] people)
        {
            throw new NotImplementedException();
        }
        public void InsertBeneficiary(params Beneficiary[] beneficiaries)
        {
            throw new NotImplementedException();
        }
        public void InsertDonors(params Donor[] donors)
        {
            throw new NotImplementedException();
        }
        public void InsertReceipeients(params Recepient[] recepients)
        {
            throw new NotImplementedException();
        }
        public void InsertVolunteers(params Volunteer[] volunteers)
        {
            throw new NotImplementedException();
        }
        public void InsertEmployee(params Employee[] employees)
        {
            throw new NotImplementedException();
        }
        public void InsertCampaign(params Campaign[] campaigns)
        {
            throw new NotImplementedException();
        }
        public void InsertCategories(params Category[] categories)
        {
            throw new NotImplementedException();
        }
        public void InsertDepartments(params Department[] departments)
        {
            throw new NotImplementedException();
        }
        public void InsertItems(params Item[] items)
        {
            throw new NotImplementedException();
        }
        public void LinkItemWithDonor(DonorItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"insert into  Donate_to (Donor_SSN,Campaign_ID,ItemMainName,ItemName,ItemSubName)" +
                $" Values({item.Donor.SSN},{item.Campaign.ID},{item.Item.Main.Name},{item.Item.Name},{item.Item.Sub.Name})",
                CommandType = CommandType.Text
            };
             cmd.ExecuteNonQuery();
        }
        public void LinkItemWithRecepient(RecepientItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"insert into  Receives_From (Recipient_SSN,Campaign_ID,ItemMainName,ItemName,ItemSubName)" +
                $" Values({item.Recipient.SSN},{item.Campaign.ID},{item.Item.Main.Name},{item.Item.Name},{item.Item.Sub.Name})",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void SetCampaignManager(Campaign campaign, Employee employee)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"update Campaign set Employee_SSN={employee.SSN} where ID_={campaign.ID} ",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void UpdateEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(DonorItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"update Donate_to set Count_={item.Count}" +
                $"where ItemName={item.Item.Name},MainName={item.Item.Main.Name},SubName={item.Item.Sub.Name}",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void UpdateLink(RecepientItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText =$"update Donate_to set Count_={item.Count}" +
                $"where ItemName={item.Item.Name},MainName={item.Item.Main.Name},SubName={item.Item.Sub.Name}",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void RecordVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void RecordBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void SetEmployeeDepartment(Employee employee, Department department)
        {
            throw new NotImplementedException();
        }
        public void SetCategoryAsMain(Category category)
        {
            throw new NotImplementedException();
        }
        public void SetCategoryAsSub(Category category, MainCategory mainCategory)
        {
            throw new NotImplementedException();
        }
        public void DeleteEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void DeleteLink(DonorItem item)
        {
             OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"delete from donate_to where donar_ssn=:ssn and
                                ItemMainName=:MainName and  ItemSubName=:SubName and 
                                itemName=:name";
            cmd.Parameters.Add("ssn", item.Donor.SSN);
            cmd.Parameters.Add("MainName", item.Item.Main.Name);
            cmd.Parameters.Add("SubName", item.Item.Sub.Name);
            cmd.Parameters.Add("name", item.Item.Name);
            cmd.ExecuteNonQuery();
        }
        public void DeleteLink(RecepientItem item)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = @"delete from receives_from  recipient_ssn=:ssn and
                                itemMainName=:MainName and  itemSubName=:SubName and 
                               where itemName=:name";
            cmd.Parameters.Add("ssn", item.Recipient.SSN);
            cmd.Parameters.Add("MainName", item.Item.Main.Name);
            cmd.Parameters.Add("SubName", item.Item.Sub.Name);
            cmd.Parameters.Add("name", item.Item.Name);
            cmd.ExecuteNonQuery();
        }
        public void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from volunteer_in where volunteer_SSN=:ssn and campaign_ID=:campaignID ";
            cmd.Parameters.Add("ssn", volunteer.SSN);
            cmd.Parameters.Add("campaignID", campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from benefit_from where beneficiary_SSN=:ssn and campaign_ID=:campaignID ";
            cmd.Parameters.Add("ssn", beneficiary.SSN);
            cmd.Parameters.Add("campaignID", campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void UnSetCategoryAsMain(MainCategory category)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from maincategory where name_=:category ";
            cmd.Parameters.Add("category", category.Name);
            cmd.ExecuteNonQuery();
        }
        public void UnSetCategoryAsSub(SubCategory category)
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "delete from subcategory where name_=:category ";
            cmd.Parameters.Add("category", category.Name);
            cmd.ExecuteNonQuery();
        }
    }
}
