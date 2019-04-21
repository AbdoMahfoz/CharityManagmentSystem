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
