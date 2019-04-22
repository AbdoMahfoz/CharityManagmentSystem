using System;
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
            for (int i = 0; i < reader.FieldCount; i++)
            {
                list[i] = reader.GetName(i);
            }
            T res = new T();
            foreach (var Property in typeof(T).GetFields())
            {
                string x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == Property.Name));
                if (!string.IsNullOrEmpty(x))
                {
                    Property.SetValue(res, reader[x]);
                }
            }
            if(typeof(T) == typeof(Item))
            {
                string x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == "MainName"));
                ((Item)(object)res).Main = FillList<MainCategory>(@"select * from MainCategory mc, Category c 
                                                                    where mc.Name_ = c.Name_
                                                                    and mc.Name_ = :n", 
                                                                    new KeyValuePair<string, object>("n", reader[x]))[0];
                x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == "SubName"));
                ((Item)(object)res).Sub = FillList<SubCategory>(@"select * from SubCategory mc, Category c 
                                                                  where mc.Name_ = c.Name_
                                                                  and mc.Name_ = :n",
                                                                  new KeyValuePair<string, object>("n", reader[x]))[0];
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
            return FillList<Department>("select dept_name,description from departmrnt dep , Employee emp where dep.dept_name=emp.department_name");
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            //Example
            FillList<Donor>("Some select query",
                            new KeyValuePair<string, object>("hi", 2),
                            new KeyValuePair<string, object>("hello", DateTime.Now);
            throw new NotImplementedException();
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
        }
        public Employee GetEmployeeManaging(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsDonatedBy(Donor donor)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsIn(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsOf(MainCategory mainCategory)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
        {
            throw new NotImplementedException();
        }
        public RecepientItem[] GetRecepientsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
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
            for (int i = 0; i < people.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into Person values (:SSN,:Name,:Mail)"
                };
                cmd.Parameters.Add("SSN", people[i].SSN);
                cmd.Parameters.Add("Name", people[i].Name);
                cmd.Parameters.Add("Mail", people[i].Mail);
                cmd.ExecuteNonQuery();
                //insert the person locations
                for (int j = 0; j < people[i].Location.Length; j++)
                {
                    OracleCommand cmd1 = new OracleCommand();
                    cmd1.Connection = conn;
                    cmd1.CommandText = "insert into Person_Location values (:SSN,:Location)";
                    cmd1.Parameters.Add("SSN", people[i].SSN);
                    cmd1.Parameters.Add("Location", people[i].Location[j]);
                    cmd1.ExecuteNonQuery();
                }
            }
        }
        public void InsertBeneficiary(params Beneficiary[] beneficiaries)
        {
            InsertPersons(beneficiaries);
            for (int i = 0; i < beneficiaries.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into beneficiary values (:SSN)"
                };
                cmd.Parameters.Add("SSN", beneficiaries[i].SSN);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertDonors(params Donor[] donors)
        {
            InsertPersons(donors);
            for (int i = 0; i < donors.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into donor values (:SSN)"
                };
                cmd.Parameters.Add("SSN", donors[i].SSN);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertReceipeients(params Recepient[] recepients)
        {
            InsertPersons(recepients);
            for (int i = 0; i < recepients.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into recepient values (:SSN)"
                };
                cmd.Parameters.Add("SSN", recepients[i].SSN);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertVolunteers(params Volunteer[] volunteers)
        {
            InsertPersons(volunteers);
            for (int i = 0; i < volunteers.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into volunteer values (:SSN)"
                };
                cmd.Parameters.Add("SSN", volunteers[i].SSN);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertEmployee(params Employee[] employees)
        {
            InsertPersons(employees);
            for (int i = 0; i < employees.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into employee values (:SSN,:Salary)"
                };
                cmd.Parameters.Add("SSN", employees[i].SSN);
                cmd.Parameters.Add("Salary", employees[i].Salary);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertCampaign(params Campaign[] campaigns)
        {
            for (int i = 0; i < campaigns.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into Campaign values (:ID,:Date,:Name,:Description,:Location,:Budget)"
                };
                cmd.Parameters.Add("Id", campaigns[i].ID);
                cmd.Parameters.Add("Date", campaigns[i].Date);
                cmd.Parameters.Add("Name", campaigns[i].Name);
                cmd.Parameters.Add("Description", campaigns[i].Description);
                cmd.Parameters.Add("Location", campaigns[i].Location);
                cmd.Parameters.Add("Budget", campaigns[i].Budget);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertCategories(params Category[] categories)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into category values (:Name,:Description)"
                };
                cmd.Parameters.Add("Name", categories[i].Name);
                cmd.Parameters.Add("Description", categories[i].Description);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertDepartments(params Department[] departments)
        {
            for (int i = 0; i < departments.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into Department values (:Name,:Description)"
                };
                cmd.Parameters.Add("Name", departments[i].DeptName);
                cmd.Parameters.Add("Description", departments[i].Description);
                cmd.ExecuteNonQuery();
            }
        }
        public void InsertItems(params Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = "insert into Item values (:Name,:Main,:Description,:Sub)"
                };
                cmd.Parameters.Add("Name", items[i].Name);
                cmd.Parameters.Add("Main", items[i].Main.Name);
                cmd.Parameters.Add("Description", items[i].Description);
                cmd.Parameters.Add("Sub", items[i].Sub.Name);
                cmd.ExecuteNonQuery();
            }
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
