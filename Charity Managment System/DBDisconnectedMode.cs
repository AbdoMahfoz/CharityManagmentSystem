using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CharityManagmentSystem.Models;
using System.Data;
using System.Threading;

namespace CharityManagmentSystem
{
    /// <summary>
    /// Downloads and saves tables into memory, mainuplates them in memory then uploads changes back to database
    /// </summary>
    class DBDisconnectedMode : IDBLayer
    {
        DataSet dataSet;
        readonly Dictionary<string, OracleDataAdapter> adapters;
        public DBDisconnectedMode()
        {
            adapters = new Dictionary<string, OracleDataAdapter>();
        }
        /// <summary>
        /// Flush exisiting dataSet and make a new one
        /// </summary>
        public void InitializeConnection()
        {
            TerminateConnection();
            dataSet = new DataSet();
        }
        /// <summary>
        /// Flush exisiting dataSet then clear it and the adapters
        /// </summary>
        public void TerminateConnection()
        {
            foreach(var adapter in adapters)
            {
                new OracleCommandBuilder(adapter.Value);
                adapter.Value.Update(dataSet, adapter.Key);
            }
            adapters.Clear();
            dataSet = null;
        }
        /// <summary>
        /// Check if the dataSet has the specificed table. If not, download it.
        /// </summary>
        /// <param name="tableName">The name of the table to be checked</param>
        public void FetchTable(string tableName)
        {
            if(!adapters.ContainsKey(tableName))
            {
                var adapter = new OracleDataAdapter($"SELECT * FROM {tableName}", DBGlobals.ConnectionString);
                DataSet tmp = new DataSet();
                adapter.Fill(tmp, tableName);
                lock(dataSet)
                {
                    dataSet.Merge(tmp);
                }
                lock(adapters)
                {
                    adapters.Add(tableName, adapter);
                }
            }
        }
        /// <summary>
        /// Fetches all given tables concurrently
        /// </summary>
        /// <param name="actions">Table names</param>
        void ParallelFetch(params string[] tables)
        {
            Thread[] threads = new Thread[tables.Length];
            for(int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart((object o) =>
                {
                    int j = (int)o;
                    FetchTable(tables[j]);
                }));
                threads[i].Start(i);
            }
            foreach(Thread t in threads)
            {
                t.Join();
            }
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            ParallelFetch("Beneficiary", "Person");
            var res = from entry in dataSet.Tables["Beneficiary"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry2.Field<int>("SSN")
                      select new Beneficiary()
                      {
                         Name = entry2.Field<string>("Name_"),
                         SSN = entry2.Field<int>("SSN"),
                         Mail = entry2.Field<string>("Mail")
                      };
            return res.ToArray();
        }
        public Campaign[] GetAllCampaigns()
        {
            FetchTable("Campaign");
            var res = from entry in dataSet.Tables["Campaign"].AsEnumerable()
                      select new Campaign()
                      {
                          Name = entry.Field<string>("Name_"),
                          Budget = entry.Field<int>("Budget"),
                          Date = entry.Field<DateTime>("Date_"),
                          Description = entry.Field<string>("Description_"),
                          Location = entry.Field<string>("Location_"),
                          ID = entry.Field<int>("ID_")
                      };
            return res.ToArray();
        }
        public Category[] GetAllCategories()
        {
            FetchTable("Category");
            var res = from entry in dataSet.Tables["Category"].AsEnumerable()
                      select new Category()
                      {
                          Name = entry.Field<string>("Name_"),
                          Description = entry.Field<string>("Description_")
                      };
            return res.ToArray();
        }
        public Department[] GetAllDepartments()
        {
            FetchTable("Department");
            var res = from entry in dataSet.Tables["Department"].AsEnumerable()
                      select new Department()
                      {
                          DeptName = entry.Field<string>("Dept_Name"),
                          Description = entry.Field<string>("Description")
                      };
            return res.ToArray();
        }
        public Donor[] GetAllDonors()
        {
            ParallelFetch("Donor", "Person");
            var res = from entry in dataSet.Tables["Donor"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry2.Field<int>("SSN")
                      select new Donor()
                      {
                          Name = entry2.Field<string>("Name_"),
                          SSN = entry2.Field<int>("SSN"),
                          Mail = entry2.Field<string>("Mail")
                      };
            return res.ToArray();
        }
        public Employee[] GetAllEmployees()
        {
            ParallelFetch("Employee", "Person");
            var res = from entry in dataSet.Tables["Employee"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Employee_SSN") equals entry2.Field<int>("SSN")
                      select new Employee()
                      {
                          Name = entry2.Field<string>("Name_"),
                          SSN = entry2.Field<int>("SSN"),
                          Mail = entry2.Field<string>("Mail"),
                          Salary = entry.Field<int>("Salary")
                      };
            return res.ToArray();
        }
        public Item[] GetAllItems()
        {
            ParallelFetch("Item", "MainCategory", "SubCategory", "Category");
            var res = from entry in dataSet.Tables["Item"].AsEnumerable()
                      select new Item()
                      {
                          Name = entry.Field<string>("Name_"),
                          Description = entry.Field<string>("Description_"),
                          Main = (from entry2 in dataSet.Tables["MainCategory"].AsEnumerable()
                                  join entry3 in dataSet.Tables["Category"].AsEnumerable()
                                  on entry2.Field<string>("Name_") equals entry3.Field<string>("Name_")
                                  where entry2.Field<string>("Name_") == entry.Field<string>("MainName")
                                  select new MainCategory()
                                  {
                                      Name = entry.Field<string>("MainName"),
                                      Description = entry3.Field<string>("Descripiton_")
                                  }).SingleOrDefault(),
                          Sub = (from entry2 in dataSet.Tables["SubCategory"].AsEnumerable()
                                 join entry3 in dataSet.Tables["Category"].AsEnumerable()
                                 on entry2.Field<string>("Name_") equals entry3.Field<string>("Name_")
                                 where entry2.Field<string>("Name_") == entry.Field<string>("MainName")
                                 select new SubCategory()
                                 {
                                     Name = entry.Field<string>("MainName"),
                                     Description = entry3.Field<string>("Descripiton_")
                                 }).SingleOrDefault()
                      };
            return res.ToArray();
        }
        public MainCategory[] GetAllMainCategories()
        {
            ParallelFetch("MainCategory", "Category");
            var res = from entry in dataSet.Tables["MainCategory"].AsEnumerable()
                      join entry2 in dataSet.Tables["Category"].AsEnumerable()
                      on entry.Field<string>("Name_") equals entry2.Field<string>("Name_")
                      select new MainCategory()
                      {
                          Name = entry2.Field<string>("Name_"),
                          Description = entry2.Field<string>("Descripiton_")
                      };
            return res.ToArray();
        }
        public Person[] GetAllPersons()
        {
            FetchTable("Person");
            var res = from entry in dataSet.Tables["Person"].AsEnumerable()
                      select new Person()
                      {
                          Name = entry.Field<string>("Name_"),
                          Mail = entry.Field<string>("Mail"),
                          SSN = entry.Field<int>("SSN")
                      };
            return res.ToArray();
        }
        public Recepient[] GetAllRecepients()
        {
            ParallelFetch("Recepient", "Person");
            var res = from entry in dataSet.Tables["Recepient"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Recepient_SSN") equals entry2.Field<int>("SSN")
                      select new Recepient()
                      {
                          Name = entry2.Field<string>("Name_"),
                          SSN = entry2.Field<int>("SSN"),
                          Mail = entry2.Field<string>("Mail")
                      };
            return res.ToArray();
        }
        public SubCategory[] GetAllSubCategories()
        {
            ParallelFetch("SubCategory", "Category");
            var res = from entry in dataSet.Tables["SubCategory"].AsEnumerable()
                      join entry2 in dataSet.Tables["Category"].AsEnumerable()
                      on entry.Field<string>("Name_") equals entry2.Field<string>("Name_")
                      select new SubCategory()
                      {
                          Name = entry2.Field<string>("Name_"),
                          Description = entry2.Field<string>("Descripiton_")
                      };
            return res.ToArray();
        }
        public Volunteer[] GetAllVolunteers()
        {
            ParallelFetch("Volunteer", "Person");
            var res = from entry in dataSet.Tables["Volunteer"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Volunteer_SSN") equals entry2.Field<int>("SSN")
                      select new Volunteer()
                      {
                          Name = entry2.Field<string>("Name_"),
                          SSN = entry2.Field<int>("SSN"),
                          Mail = entry2.Field<string>("Mail")
                      };
            return res.ToArray();
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            ParallelFetch("Beneficiary", "Person", "Benefit_From");
            var res = from entry in dataSet.Tables["Beneficiary"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry2.Field<int>("SSN")
                      join entry3 in dataSet.Tables["Benefit_From"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry3.Field<int>("Beneficiary_SSN")
                      where entry3.Field<int>("Campaign_ID") == campaign.ID
                      select new Beneficiary()
                      {
                          Name = entry2.Field<string>("Name_"),
                          Mail = entry2.Field<string>("Mail"),
                          SSN = entry2.Field<int>("SSN")
                      };
            return res.ToArray();
        }
        public Department GetDepartmentOf(Employee employee)
        {
            ParallelFetch("Employee", "Department");
            var res = from entry in dataSet.Tables["Employee"].AsEnumerable()
                      join entry2 in dataSet.Tables["Department"].AsEnumerable()
                      on entry.Field<string>("Department_Name") equals entry2.Field<string>("Dept_Name")
                      select new Department()
                      {
                          DeptName = entry2.Field<string>("Dept_Name"),
                          Description = entry2.Field<string>("Description")
                      };
            return res.Single();
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            throw new NotImplementedException();
        }
        public Employee GetEmployeeManaging(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
        }
        public Volunteer[] GetVolunteersOf(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Recepient[] GetRecepientsReceivingFrom(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public RecepientItem[] GetRecepientsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
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
        public void UpdateEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(DonorItem donorItem)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(RecepientItem recepientItem)
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
