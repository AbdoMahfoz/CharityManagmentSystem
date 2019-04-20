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
            foreach(var adapter in adapters.Values)
            {
                new OracleCommandBuilder(adapter);
                adapter.Update(dataSet);
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
                adapter.Fill(dataSet, tableName);
                adapters.Add(tableName, adapter);
            }
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            FetchTable("Beneficiary");
            FetchTable("Person");
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
            throw new NotImplementedException();
        }
        public Category[] GetAllCategories()
        {
            throw new NotImplementedException();
        }
        public Department[] GetAllDepartments()
        {
            throw new NotImplementedException();
        }
        public Donor[] GetAllDonors()
        {
            throw new NotImplementedException();
        }
        public Employee[] GetAllEmployees()
        {
            throw new NotImplementedException();
        }
        public Item[] GetAllItems()
        {
            throw new NotImplementedException();
        }
        public MainCategory[] GetAllMainCategories()
        {
            throw new NotImplementedException();
        }
        public Person[] GetAllPersons()
        {
            throw new NotImplementedException();
        }
        public Recepient[] GetAllRecepients()
        {
            throw new NotImplementedException();
        }
        public SubCategory[] GetAllSubCategories()
        {
            throw new NotImplementedException();
        }
        public Volunteer[] GetAllVolunteers()
        {
            throw new NotImplementedException();
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
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
        public void LinkCampaignWithEmployee(Campaign campaign, Employee employee)
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
        public void UpdateLink(Campaign campaign, Employee employee)
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
        public void DeleteLink(Campaign campaign, Employee employee)
        {
            throw new NotImplementedException();
        }
    }
}
