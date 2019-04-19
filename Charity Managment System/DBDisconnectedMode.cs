using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CharityManagmentSystem.Models;

namespace CharityManagmentSystem
{
    class DBDisconnectedMode : IDBLayer
    {
        public DBDisconnectedMode()
        {

        }
        public void InitializeConnection()
        {
            throw new NotImplementedException();
        }
        public void TerminateConnection()
        {
            throw new NotImplementedException();
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            throw new NotImplementedException();
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
