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
            string[] list = new string[reader.VisibleFieldCount];
            for (int i = 0; i < reader.VisibleFieldCount; i++)
            {
                list[i] = reader.GetName(i);
            }
            T res = new T();
            foreach (var Property in typeof(T).GetFields())
            {
                string x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "").ToLower() == Property.Name.ToLower()));
                if (!string.IsNullOrEmpty(x))
                {
                    Property.SetValue(res, reader[x]);
                }
            }
            if (typeof(T) == typeof(Item))
            {
                string x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == "MainName"));
                ((Item)(object)res).Main = FillList<MainCategory>(@"select * from MainCategory mc, Category_ c 
                                                                    where mc.Name_ = c.Name_
                                                                    and mc.Name_ = :n", null,
                                                                    new KeyValuePair<string, object>("n", reader[x]))[0];
                x = Array.Find(list, new Predicate<string>(
                    (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == "SubName"));
                ((Item)(object)res).Sub = FillList<SubCategory>(@"select * from SubCategory mc, Category_ c 
                                                                  where mc.Name_ = c.Name_
                                                                  and mc.Name_ = :n", null,
                                                                  new KeyValuePair<string, object>("n", reader[x]))[0];
            }
            return res;
        }
        private T[] FillList<T>(string cmdstr, string refCursor = null, params KeyValuePair<string, object>[] args) where T : new()
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = cmdstr,
                CommandType = (refCursor == null) ? CommandType.Text : CommandType.StoredProcedure
            };
            if (refCursor != null)
            {
                cmd.Parameters.Add(refCursor, OracleDbType.RefCursor, ParameterDirection.Output);
            }
            foreach (var arg in args)
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
            return FillList<Department>("GetALLDepartments", "dept");
        }
        public Donor[] GetAllDonors()
        {
            return FillList<Donor>("select * from Donor d, Person p where d.Donor_SSN = p.SSN");
        }
        public Employee[] GetAllEmployees()
        {
            return FillList<Employee>("select * from Employee e, Person p where e.Employee_SSN = p.SSN");
        }
        public Item[] GetAllItems()
        {
            return FillList<Item>("select * from Item");
        }
        public MainCategory[] GetAllMainCategories()
        {
            return FillList<MainCategory>("select * from MainCategory MC ,Category_ C where MC.Name_ = C.Name_");
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
            return FillList<SubCategory>("select * from SubCategory sub,category_ c where sub.name_=c.name_");
        }
        public Volunteer[] GetAllVolunteers()
        {
            return FillList<Volunteer>("select * from Volunteer v,Person p where v.volunteer_ssn=p.ssn");
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            return FillList<Beneficiary>(@"select beneficiary_ssn from Beneficiary b ,Benefit_from bf 
                                         where b.beneficiary_ssn=bf.beneficiary_ssn and bf.campaign_id=:campaign", null,
                                         new KeyValuePair<string, object>("campaign", campaign.ID));
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
        {
            return FillList<Department>(@"select dept_name,description from department dep , Employee emp where emp.Department_Name = dep.Dept_Name
                                                                                                          and   emp.Employee_SSN = :ssn", null,
                                        new KeyValuePair<string, object>("ssn", employee.SSN));
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            return FillList<Donor>(@"select * from Donate_to DT, Donor D, Person P 
                                    where DT.Campaign_ID = :IDT and DT.Donor_SSN = D.Donor_SSN and D.Donor_SSN = P.SSN", null,
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
                                where C.ID_ = :IDT and C.Employee_SSN = E.Employee_SSN and E.Employee_SSN = P.SSN", null,
                                new KeyValuePair<string, object>("IDT", campaign.ID))[0];
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            return FillList<Employee>(@"select * from Employee E , Person P 
                                        where E.Department_Name = :DN and E.Employee_SSN = P.SSN", null,
                            new KeyValuePair<string, object>("DN", department.DeptName));
        }
        public Item[] GetItemsDonatedBy(Donor donor)
        {
            return FillList<Item>("select * from Donate_to DT where DT.Donor_SSN = :DN", null,
                            new KeyValuePair<string, object>("DN", donor.SSN));
        }
        public Item[] GetItemsIn(Campaign campaign)
        {
            return FillList<Item>("select * from Donate_to RF where RF.Campaign_ID = :IDT", null,
                            new KeyValuePair<string, object>("IDT", campaign.ID));
        }
        public Item[] GetItemsOf(MainCategory mainCategory)
        {
            return FillList<Item>("select * from Item I where I.ItemMainName = :IMN", null,
                            new KeyValuePair<string, object>("IMN", mainCategory.Name));
        }
        public Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory)
        {
            return FillList<Item>("select * from Item I where I.ItemMainName = :IMN and I.ItemSubName = :ISN", null,
                            new KeyValuePair<string, object>("IMN", mainCategory.Name),
                            new KeyValuePair<string, object>("ISN", subCategory.Name));
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
        {
            return FillList<Item>("select * from Receives_From RF where RF.Recipient_SSN = :RSSN", null,
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
        public Recepient[] GetRecepientsReceivingFrom(Campaign campaign)
        {
            return FillList<Recepient>(@"select * from recepient r , Receives_From rf, Person p
                                         where p.SSN = r.recepient_ssn and r.recipient_ssn = rf.Recipient_SSN and rf.Campaign_ID =:campaign", null,
                                         new KeyValuePair<string, object>("campaign", campaign.ID));
        }
        public Volunteer[] GetVolunteersOf(Campaign campaign)
        {
            return FillList<Volunteer>(@"select * from volunteer v , volunteer_in Vin, Person p
                                         where p.SSN = v.volunteer_ssn and v.volunteer_ssn = Vin.volunteer_SSN and Vin.Campaign_ID =:campaign", null,
                                        new KeyValuePair<string, object>("campaign", campaign.ID));
        }
        public Department GetDepartmentOf(Employee employee)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "GetDepartmentOf",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("SSN", employee.SSN);
            cmd.Parameters.Add("Name_", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.Parameters.Add("description", OracleDbType.Varchar2, ParameterDirection.Output);
            cmd.ExecuteNonQuery();
            return new Department()
            {
                DeptName = cmd.Parameters["Name_"].Value as string,
                Description = cmd.Parameters["description"].Value as string
            };
        }
        public Campaign[] GetCampaginsManagedBy(Employee employee)
        {
            return FillList<Campaign>(@"select * from campaign c where c.employee_ssn=:emp_ssn", null,
                                       new KeyValuePair<string, object>("emp_ssn", employee.SSN));
        }
        public Campaign[] GetCampaignsOf(Volunteer volunteer)
        {
            return FillList<Campaign>(@"select * from campaign c ,volunteer_in Vin where c.id_=Vin.Campaign_ID 
                                      and Vin.volunteer_ssn=:volunteerSSN ", null,
                                      new KeyValuePair<string, object>("volunteerSSN", volunteer.SSN));
        }
        public Campaign[] GetCampaignsOf(Donor donor)
        {
            return FillList<Campaign>(@"select * from campaign c , donate_to D where c.id_=v.Campaign_ID 
                                      and d.donor_ssn=:donorSSN ", null,
                                      new KeyValuePair<string, object>("donorSSN", donor.SSN));
        }
        public Campaign[] GetCampaignsOf(Recepient recepient)
        {
            return FillList<Campaign>(@"select * from campaign c , Receives_From rf where c.id_=rf.Campaign_ID 
                                      and rf.Recipient_SSN=:recipientSSN ", null,
                                      new KeyValuePair<string, object>("recipientSSN", recepient.SSN));
        }
        public Campaign[] GetCampaignsOf(Beneficiary beneficiary)
        {
            return FillList<Campaign>(@"select * from campaign c , Benefit_from bf where c.id_=bf.Campaign_ID 
                                      and bf.Beneficiary_SSN=:BeneficiarySSN ", null,
                                      new KeyValuePair<string, object>("BeneficiarySSN", beneficiary.SSN));
        }
        public DonorItem[] GetDonorsOf(Item item)
        {
            return FillList<DonorItem>(@"select * from donor d , donate_to dt where d.donor_ssn=dt.donor_ssn and 
                                       dt.itemname=:item", null,
                                       new KeyValuePair<string, object>("item", item.Name));
        }
        public SubCategory[] GetSubCategoriesOf(MainCategory mainCategory)
        {
            return FillList<SubCategory>(@"select * from SubCategory where Main_Name = :name", null,
                                         new KeyValuePair<string, object>("name", mainCategory.Name));
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
                for (int j = 0; j < people[i].Location.Length; j++)
                {
                    OracleCommand cmd1 = new OracleCommand
                    {
                        Connection = conn,
                        CommandText = "insert into Person_Location values (:SSN,:Location)"
                    };
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
                    CommandText = "insert into Campaign (ID_, Date_, Name_, Description_, Location_, Budget) values (:ID_,:Date_,:Name_,:Description,:Location,:Budget)"
                };
                cmd.Parameters.Add("ID_", campaigns[i].ID);
                cmd.Parameters.Add("Date_", campaigns[i].Date);
                cmd.Parameters.Add("Name_", campaigns[i].Name);
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
                    CommandText = "insertdep",
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.Add("Name_", departments[i].DeptName);
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
        public void UpdateCampaignManager(Campaign campaign, Employee employee)
        {
            string query = $"UPDATE Campaign " +
                           $"SET Employee_SSN = {employee.SSN} " +
                           $"WHERE ID_ = {campaign.ID}";
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = query,
                CommandType = CommandType.Text
            };
            if (cmd.ExecuteNonQuery() != 1)
            {
                throw new Exception("Invalid entity: no rows got updated");
            }
        }
        public void UpdateEntity<T>(T Entity)
        {
            void ExecuteCommand(string query, CommandType type = CommandType.Text, params KeyValuePair<string, object>[] args)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = query,
                    CommandType = type
                };
                foreach (var arg in args)
                {
                    cmd.Parameters.Add(arg.Key, arg.Value);
                }
                if (cmd.ExecuteNonQuery() != 1)
                {
                    throw new Exception("Invalid entity: no rows got updated");
                }
            }
            if (Entity is Person person)
            {
                ExecuteCommand($"UPDATE Person " +
                                $"SET Name_ = {person.Name}, Mail = {person.Mail} " +
                                $"WHERE SSN = {person.SSN} ");
            }
            if (Entity is Department department)
            {
                ExecuteCommand($"UPDATE Department " +
                                $"SET Description = {department.Description} " +
                                $"WHERE Dept_Name = {department.DeptName} ");
            }
            if (Entity is Employee employee)
            {
                ExecuteCommand($"UPDATE Employee " +
                                $"SET Salary = {employee.Salary} " +
                                $"WHERE Employee_SSN = {employee.SSN}");
            }
            if (Entity is Campaign campaign)
            {
                ExecuteCommand("update_campaign", CommandType.StoredProcedure,
                               new KeyValuePair<string, object>("Camp_ID", campaign.ID),
                               new KeyValuePair<string, object>("Date_", campaign.Date),
                               new KeyValuePair<string, object>("Name_", campaign.Name),
                               new KeyValuePair<string, object>("description", campaign.Description),
                               new KeyValuePair<string, object>("Location_", campaign.Location),
                               new KeyValuePair<string, object>("Budget", campaign.Budget));
            }
            if (Entity is Category category)
            {
                ExecuteCommand($"UPDATE Category_ " +
                                $"SET Description_ = {category.Description} " +
                                $"WHERE Name_ = {category.Name}");
            }
            if (Entity is Item item)
            {
                ExecuteCommand($"UPDATE Item " +
                               $"SET Description_ = {item.Description} " +
                               $"WHERE Name_ = {item.Name}, MainName = {item.Main.Name}, SubName = {item.Sub.Name}");
            }
            if (Entity is DonorItem donorItem)
            {
                ExecuteCommand($"UPDATE Donate_to " +
                               $"SET Count_ = {donorItem.Count} " +
                               $"WHERE Donor_SSN = {donorItem.Donor.SSN}, Campaign_ID = {donorItem.Campaign.ID}, " +
                               $"      ItemName = {donorItem.Item.Name}, ItemMainName = {donorItem.Item.Main.Name}, ItemSubName = {donorItem.Item.Sub.Name}");
            }
            if (Entity is RecepientItem recipientItem)
            {
                ExecuteCommand($"UPDATE Receives_From " +
                               $"SET Count_ = {recipientItem.Count} " +
                               $"WHERE Recipient_SSN = {recipientItem.Recipient.SSN}, Campaign_ID = {recipientItem.Campaign.ID}, " +
                               $"      ItemName = {recipientItem.Item.Name}, ItemMainName = {recipientItem.Item.Main.Name} " +
                               $"                                          , ItemSubName = {recipientItem.Item.Sub.Name}");
            }
        }
        public void UpdateLink(DonorItem item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"update Donate_to set Count_={item.Count}" +
                $"where ItemName={item.Item.Name}, ItemMainName={item.Item.Main.Name}," +
                $" ItemSubName={item.Item.Sub.Name},Donor_SSN={item.Donor.SSN},Campaign_ID={item.Campaign.ID}",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void UpdateLink(RecepientItem item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"update Receives_From set Count_={item.Count}" +
                $"where ItemName={item.Item.Name}, ItemMainName={item.Item.Main.Name}, " +
                $"ItemSubName={item.Item.Sub.Name},Recipient_SSN={item.Recipient.SSN},Campaign_ID={item.Campaign.ID}",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void RecordVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"insert into  Volunteer_in (Volunteer_SSN,Campaign_ID)" +
                $" Values({volunteer.SSN},{campaign.ID})",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void RecordBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"insert into  Benefit_from (Beneficiary_SSN,Campaign_ID)" +
                $" Values({beneficiary.SSN},{campaign.ID})",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void SetEmployeeDepartment(Employee employee, Department department)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"update Employee set Department_Name={department.DeptName}" +
                $"where Employee_SSN={employee.SSN}",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void SetCategoryAsMain(Category category)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"Insert into MainCategory(Name_) Values ({category.Name}) ",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void SetCategoryAsSub(Category category, MainCategory mainCategory)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = $"Insert into SubCategory (Name_,Main_Name) Values ({category.Name},{mainCategory.Name})",
                CommandType = CommandType.Text
            };
            cmd.ExecuteNonQuery();
        }
        public void DeleteEntity<T>(T Entity)
        {
            void ExecuteCommand(string query, CommandType type = CommandType.Text, params KeyValuePair<string, object>[] args)
            {
                OracleCommand cmd = new OracleCommand
                {
                    Connection = conn,
                    CommandText = query,
                    CommandType = type
                };
                foreach (var arg in args)
                {
                    cmd.Parameters.Add(arg.Key, arg.Value);
                }
                cmd.ExecuteNonQuery();
            }
            if (Entity is Person person)
            {
                Dictionary<Type, string> tables = new Dictionary<Type, string>()
                {
                    { typeof(Volunteer), "Volunteer_in" },
                    { typeof(Beneficiary), "Benefit_from" },
                    { typeof(Donor), "Donate_to" },
                    { typeof(Recepient), "Receives_From" }
                };
                if (Entity is Employee)
                {
                    ExecuteCommand($"UPDATE Campaign SET Employee_SSN = NULL WHERE Employe_SSN = {person.SSN}");
                }
                else if (tables.TryGetValue(typeof(T), out string table))
                {
                    ExecuteCommand($"DELETE FROM {table} WHERE {typeof(T).Name}_SSN = {person.SSN}");
                }
                ExecuteCommand($"DELETE FROM {typeof(T).Name} WHERE {typeof(T).Name}_SSN = {person.SSN}");
                ExecuteCommand($"DELETE FROM Person WHERE SSN = {person.SSN}");
            }
            if (Entity is Department department)
            {
                ExecuteCommand($"UPDATE Employee SET Department_Name = NULL WHERE Department_Name = {department.DeptName}");
                ExecuteCommand($"DELETE FROM Department WHERE Dept_Name = {department.DeptName}");
            }
            if (Entity is Campaign campaign)
            {
                ExecuteCommand($"DELETE FROM Donate_to WHERE Campaign_ID = {campaign.ID}");
                ExecuteCommand($"DELETE FROM Receives_From WHERE Campaign_ID = {campaign.ID}");
                ExecuteCommand($"DELETE FROM Campaign WHERE ID_ = {campaign.ID}");
            }
            if (Entity is Category category)
            {
                if (Entity is MainCategory mainCategory)
                {
                    ExecuteCommand($"DELETE FROM Donate_to WHERE ItemMainName = {mainCategory.Name}");
                    ExecuteCommand($"DELETE FROM Receives_From WHERE ItemMainName = {mainCategory.Name}");
                    ExecuteCommand($"DELETE FROM Item WHERE MainName = {mainCategory.Name}");
                    ExecuteCommand($"DELETE FROM SubCategory WHERE Main_Name = {mainCategory.Name}");
                    ExecuteCommand($"DELETE FROM MainCategory WHERE Name_ = {mainCategory.Name}");
                }
                else if (Entity is SubCategory subCategory)
                {
                    ExecuteCommand($"DELETE FROM Donate_to WHERE ItemSubName = {subCategory.Name}");
                    ExecuteCommand($"DELETE FROM Receives_From WHERE ItemSubName = {subCategory.Name}");
                    ExecuteCommand($"DELETE FROM Item WHERE SubName = {subCategory.Name}");
                    ExecuteCommand($"DELETE FROM SubCategory WHERE Name_ = {subCategory.Name}");
                }
                ExecuteCommand($"DELETE FROM Category_ WHERE Name_ = {category.Name}");
            }
            if (Entity is Item item)
            {
                ExecuteCommand($"DELETE FROM Receives_From WHERE ItemName = {item.Name}," +
                               $"                                ItemMainName = {item.Main.Name}, ItemSubName = {item.Sub.Name}");
                ExecuteCommand($"DELETE FROM Donate_to WHERE ItemName = {item.Name}," +
                               $"                            ItemMainName = {item.Main.Name}, ItemSubName = {item.Sub.Name}");
                ExecuteCommand($"DELETE FROM Item WHERE Name_ = {item.Name}," +
                               $"                       MainName = {item.Main.Name}, SubName = {item.Sub.Name}");
            }
        }
        public void DeleteLink(DonorItem item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = @"delete from donate_to where donar_ssn=:ssn and
                                ItemMainName=:MainName and  ItemSubName=:SubName and 
                                itemName=:name and Campaign_ID=:ID"
            };
            cmd.Parameters.Add("ssn", item.Donor.SSN);
            cmd.Parameters.Add("MainName", item.Item.Main.Name);
            cmd.Parameters.Add("SubName", item.Item.Sub.Name);
            cmd.Parameters.Add("name", item.Item.Name);
            cmd.Parameters.Add("ID", item.Campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void DeleteLink(RecepientItem item)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = @"delete from receives_from where recipient_ssn=:ssn and
                                itemMainName=:MainName and  itemSubName=:SubName and 
                                itemName=:name and Campaign_ID=:ID"
            };
            cmd.Parameters.Add("ssn", item.Recipient.SSN);
            cmd.Parameters.Add("MainName", item.Item.Main.Name);
            cmd.Parameters.Add("SubName", item.Item.Sub.Name);
            cmd.Parameters.Add("name", item.Item.Name);
            cmd.Parameters.Add("ID", item.Campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "delete from volunteer_in where volunteer_SSN=:ssn and campaign_ID=:campaignID "
            };
            cmd.Parameters.Add("ssn", volunteer.SSN);
            cmd.Parameters.Add("campaignID", campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "DeleteBeneficiaryParticipation",
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add("ssn", beneficiary.SSN);
            cmd.Parameters.Add("camp_ID", campaign.ID);
            cmd.ExecuteNonQuery();
        }
        public void UnSetCategoryAsMain(MainCategory category)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "delete from maincategory where name_=:category "
            };
            cmd.Parameters.Add("category", category.Name);
            cmd.ExecuteNonQuery();
        }
        public void UnSetCategoryAsSub(SubCategory category)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "delete from subcategory where name_=:category "
            };
            cmd.Parameters.Add("category", category.Name);
            cmd.ExecuteNonQuery();
        }
        public DataTable GetTable(string value, TableType tableType = TableType.Predefined)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = (tableType == TableType.Predefined)? $"SELECT * FROM {value}" : value
            };
            var reader = cmd.ExecuteReader();
            DataTable table = new DataTable((tableType == TableType.Predefined) ? value : null);
            for(int i = 0; i < reader.VisibleFieldCount; i++)
            {
                table.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
            }
            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for (int i = 0; i < reader.VisibleFieldCount; i++)
                {
                    row[reader.GetName(i)] = reader[i];
                }
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
