using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharityManagmentSystem.Models;

namespace CharityManagmentSystem
{
    public enum DataAccessMode { Connected, Disconnected };
    public class CMS : IDBLayer
    {
        IDBLayer dbLayer;
        DataAccessMode CurrentMode;
        public CMS(DataAccessMode mode)
        {
            CurrentMode = mode;
            if(mode == DataAccessMode.Connected)
            {
                dbLayer = new DBConnectedMode();
            }
            else
            {
                dbLayer = new DBDisconnectedMode();
            }
        }
        public void SwitchStrategy()
        {
            CurrentMode = (CurrentMode == DataAccessMode.Connected) ? DataAccessMode.Disconnected : DataAccessMode.Connected;
            TerminateConnection();
            if (CurrentMode == DataAccessMode.Connected)
            {
                dbLayer = new DBConnectedMode();
            }
            else
            {
                dbLayer = new DBDisconnectedMode();
            }
            InitializeConnection();
        }
        public void SwitchStrategy(DataAccessMode mode)
        {
            if(CurrentMode != mode)
            {
                CurrentMode = mode;
                TerminateConnection();
                if (mode == DataAccessMode.Connected)
                {
                    dbLayer = new DBConnectedMode();
                }
                else
                {
                    dbLayer = new DBDisconnectedMode();
                }
                InitializeConnection();
            }
        }
        public void DeleteEntity<T>(T Entity)
        {
            dbLayer.DeleteEntity(Entity);
        }
        public void DeleteLink(DonorItem item)
        {
            dbLayer.DeleteLink(item);
        }
        public void DeleteLink(RecepientItem item)
        {
            dbLayer.DeleteLink(item);
        }
        public void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            dbLayer.EraseBeneficiaryParticipation(beneficiary, campaign);
        }
        public void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            dbLayer.EraseVolunteerParticipation(volunteer, campaign);
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            return dbLayer.GetAllBeneficiaries();
        }
        public Campaign[] GetAllCampaigns()
        {
            return dbLayer.GetAllCampaigns();
        }
        public Category[] GetAllCategories()
        {
            return dbLayer.GetAllCategories();
        }
        public Department[] GetAllDepartments()
        {
            return dbLayer.GetAllDepartments();
        }
        public Donor[] GetAllDonors()
        {
            return dbLayer.GetAllDonors();
        }
        public Employee[] GetAllEmployees()
        {
            return dbLayer.GetAllEmployees();
        }
        public Item[] GetAllItems()
        {
            return dbLayer.GetAllItems();
        }
        public MainCategory[] GetAllMainCategories()
        {
            return dbLayer.GetAllMainCategories();
        }
        public Person[] GetAllPersons()
        {
            return dbLayer.GetAllPersons();
        }
        public Recepient[] GetAllRecepients()
        {
            return dbLayer.GetAllRecepients();
        }
        public SubCategory[] GetAllSubCategories()
        {
            return dbLayer.GetAllSubCategories();
        }
        public Volunteer[] GetAllVolunteers()
        {
            return dbLayer.GetAllVolunteers();
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            return dbLayer.GetBeneficiariesOf(campaign);
        }
        public Campaign[] GetCampaginsManagedBy(Employee employee)
        {
            return dbLayer.GetCampaginsManagedBy(employee);
        }
        public Campaign[] GetCampaignsOf(Volunteer volunteer)
        {
            return dbLayer.GetCampaignsOf(volunteer);
        }
        public Campaign[] GetCampaignsOf(Donor donor)
        {
            return dbLayer.GetCampaignsOf(donor);
        }
        public Campaign[] GetCampaignsOf(Recepient recepient)
        {
            return dbLayer.GetCampaignsOf(recepient);
        }
        public Campaign[] GetCampaignsOf(Beneficiary beneficiary)
        {
            return dbLayer.GetCampaignsOf(beneficiary);
        }
        public Department GetDepartmentOf(Employee employee)
        {
            return dbLayer.GetDepartmentOf(employee);
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            return dbLayer.GetDonorsDonatingTo(campaign);
        }
        public DonorItem[] GetDonorsOf(Item item)
        {
            return dbLayer.GetDonorsOf(item);
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            return dbLayer.GetDonorsOf(campaign, item);
        }
        public Employee GetEmployeeManaging(Campaign campaign)
        {
            return dbLayer.GetEmployeeManaging(campaign);
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            return dbLayer.GetEmployeesWorkingIn(department);
        }
        public Item[] GetItemsDonatedBy(Donor donor)
        {
            return dbLayer.GetItemsDonatedBy(donor);
        }
        public Item[] GetItemsIn(Campaign campaign)
        {
            return dbLayer.GetItemsIn(campaign);
        }
        public Item[] GetItemsOf(MainCategory mainCategory)
        {
            return dbLayer.GetItemsOf(mainCategory);
        }
        public Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory)
        {
            return dbLayer.GetItemsOf(mainCategory, subCategory);
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
        {
            return dbLayer.GetItemsReceivedBy(recepient);
        }
        public RecepientItem[] GetRecepientsOf(Campaign campaign, Item item)
        {
            return dbLayer.GetRecepientsOf(campaign, item);
        }
        public Recepient[] GetRecepientsReceivingFrom(Campaign campaign)
        {
            return dbLayer.GetRecepientsReceivingFrom(campaign);
        }
        public SubCategory[] GetSubCategoriesOf(MainCategory mainCategory)
        {
            return dbLayer.GetSubCategoriesOf(mainCategory);
        }
        public Volunteer[] GetVolunteersOf(Campaign campaign)
        {
            return dbLayer.GetVolunteersOf(campaign);
        }
        public void InitializeConnection()
        {
            dbLayer.InitializeConnection();
        }
        public void InsertBeneficiary(params Beneficiary[] beneficiaries)
        {
            dbLayer.InsertBeneficiary(beneficiaries);
        }
        public void InsertCampaign(params Campaign[] campaigns)
        {
            dbLayer.InsertCampaign(campaigns);
        }
        public void InsertCategories(params Category[] categories)
        {
            dbLayer.InsertCategories(categories);
        }
        public void InsertDepartments(params Department[] departments)
        {
            dbLayer.InsertDepartments(departments);
        }
        public void InsertDonors(params Donor[] donors)
        {
            dbLayer.InsertDonors(donors);
        }
        public void InsertEmployee(params Employee[] employees)
        {
            dbLayer.InsertEmployee(employees);
        }
        public void InsertItems(params Item[] items)
        {
            dbLayer.InsertItems(items);
        }
        public void InsertPersons(params Person[] people)
        {
            dbLayer.InsertPersons(people);
        }
        public void InsertReceipeients(params Recepient[] recepients)
        {
            dbLayer.InsertReceipeients(recepients);
        }
        public void InsertVolunteers(params Volunteer[] volunteers)
        {
            dbLayer.InsertVolunteers(volunteers);
        }
        public void LinkItemWithDonor(DonorItem item)
        {
            dbLayer.LinkItemWithDonor(item);
        }
        public void LinkItemWithRecepient(RecepientItem item)
        {
            dbLayer.LinkItemWithRecepient(item);
        }
        public void RecordBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            dbLayer.RecordBeneficiaryParticipation(beneficiary, campaign);
        }
        public void RecordVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            dbLayer.RecordVolunteerParticipation(volunteer, campaign);
        }
        public void SetCampaignManager(Campaign campaign, Employee employee)
        {
            dbLayer.SetCampaignManager(campaign, employee);
        }
        public void SetCategoryAsMain(Category category)
        {
            dbLayer.SetCategoryAsMain(category);
        }
        public void SetCategoryAsSub(Category category, MainCategory mainCategory)
        {
            dbLayer.SetCategoryAsSub(category, mainCategory);
        }
        public void SetEmployeeDepartment(Employee employee, Department department)
        {
            dbLayer.SetEmployeeDepartment(employee, department);
        }
        public void TerminateConnection()
        {
            dbLayer.TerminateConnection();
        }
        public void UnSetCategoryAsMain(MainCategory category)
        {
            dbLayer.UnSetCategoryAsMain(category);
        }
        public void UnSetCategoryAsSub(SubCategory category)
        {
            dbLayer.UnSetCategoryAsSub(category);
        }
        public void UpdateEntity<T>(T Entity)
        {
            dbLayer.UpdateEntity(Entity);
        }
        public void UpdateLink(DonorItem donorItem)
        {
            dbLayer.UpdateLink(donorItem);
        }
        public void UpdateLink(RecepientItem recepientItem)
        {
            dbLayer.UpdateLink(recepientItem);
        }
    }
}
