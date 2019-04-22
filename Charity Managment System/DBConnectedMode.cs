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
        private T[] FillList<T>(string cmdstr) where T : new()
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = cmdstr,
                CommandType = CommandType.Text
            };
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
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Beneficiary";
            List<Beneficiary> benlist = new List<Beneficiary>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Beneficiary b = new Beneficiary();
                b = FillObject<Beneficiary>(dr);
                benlist.Add(b);

            }
            dr.Close();
            return benlist.ToArray();
        }
        public Campaign[] GetAllCampaigns()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Campaign";
            List<Campaign> camplist = new List<Campaign>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Campaign b = new Campaign();
                b = FillObject<Campaign>(dr);
                camplist.Add(b);

            }
            dr.Close();
            return camplist.ToArray();
        }
        public Category[] GetAllCategories()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Category";
            List<Category> catlist = new List<Category>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Category b = new Category();
                b = FillObject<Category>(dr);
                catlist.Add(b);

            }
            dr.Close();
            return catlist.ToArray();
        }
        public Department[] GetAllDepartments()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Department";
            List<Department> deplist = new List<Department>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Department b = new Department();
                b = FillObject<Department>(dr);
                deplist.Add(b);

            }
            dr.Close();
            return deplist.ToArray();
        }
        public Donor[] GetAllDonors()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Donor";
            List<Donor> donlist = new List<Donor>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Donor b = new Donor();
                b = FillObject<Donor>(dr);
                donlist.Add(b);

            }
            dr.Close();
            return donlist.ToArray();
        }
        public Employee[] GetAllEmployees()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Employee";
            List<Employee> emplist = new List<Employee>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Employee b = new Employee();
                b = FillObject<Employee>(dr);
                emplist.Add(b);

            }
            dr.Close();
            return emplist.ToArray();
        }
        public Item[] GetAllItems()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Item";
            List<Item> itemlist = new List<Item>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Item b = new Item();
                b = FillObject<Item>(dr);
                itemlist.Add(b);

            }
            dr.Close();
            return itemlist.ToArray();
        }
        public MainCategory[] GetAllMainCategories()
        {
            return FillList<MainCategory>("select * from MainCategory");
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
            return FillList<Beneficiary>("select beneficiary_ssn from Beneficiary b ,Benefit_from bf , Campaign C " +
                                         "where b.beneficiary_ssn=bf.beneficiary_ssn and bf.campaign_id=C.campaign_id");
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
        {
            return FillList<Department>("select dept_name,description from departmrnt dep , Employee emp where dep.dept_name=emp.department_name");
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
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
            throw new NotImplementedException();
        }
        public void LinkItemWithRecepient(RecepientItem item)
        {
            throw new NotImplementedException();
        }
        public void SetCampaignManager(Campaign campaign, Employee employee)
        {
            throw new NotImplementedException();
        }
        public void UpdateEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(DonorItem item)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(RecepientItem item)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
        public void DeleteLink(RecepientItem item)
        {
            throw new NotImplementedException();
        }
        public void FireEmployeeFromDepartment(Employee employee, Department department)
        {
            throw new NotImplementedException();
        }
        public void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void UnSetCategoryAsMain(MainCategory category)
        {
            throw new NotImplementedException();
        }
        public void UnSetCategoryAsSub(SubCategory category)
        {
            throw new NotImplementedException();
        }
    }
}
