using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using Charity_Managment_System.Models;

namespace Charity_Managment_System
{
    class DBConnectedMode : IDBLayer
    {
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
        public Employee[] GetEmployeesManaging(Campaign campaign)
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
        public void InitializeConnection()
        {
            throw new NotImplementedException();
        }
        public void TerminateConnection()
        {
            throw new NotImplementedException();
        }
    }
}
