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
                CommandText = "insert into  Donate_to (Donor_SSN,Campaign_ID,ItemMainName,   ,ItemSubName)" +
                " Values(Donor.Donor_SSN,Campaign.ID_,item.MainName,   ,item.ItemSubName,)",
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
        }
        public void LinkItemWithRecepient(RecepientItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "insert into  Receives_From (Recipient_SSN,Campaign_ID,ItemMainName,   ,ItemSubName)" +
                " Values(Recipient.Recipient_SSN,Campaign.ID_,item.MainName,   ,item.ItemSubName,)",
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
        }
        public void SetCampaignManager(Campaign campaign, Employee employee)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "update Campaign set Employee_SSN=:employee.Employee_SSN where ID_=:campaign.ID_ ",
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
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
                CommandText = "update Donate_to set Count_=:       " +
                "where Name_=:item.Name_,MainName=:item.MainName,SubName:=item.SubName",
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
        }
        public void UpdateLink(RecepientItem item)
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = "update Receives_From set Count_=:                  ",
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
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
