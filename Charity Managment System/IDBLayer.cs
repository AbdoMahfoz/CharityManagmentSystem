using CharityManagmentSystem.Models;

namespace CharityManagmentSystem
{
    /// <summary>
    /// Defines a list of functions that cover all interactions with the database
    /// </summary>
    interface IDBLayer
    {
        //Connection Control
        void InitializeConnection();
        void TerminateConnection();
        //Get All....
        Person[] GetAllPersons();
        Employee[] GetAllEmployees();
        Donor[] GetAllDonors();
        Volunteer[] GetAllVolunteers();
        Recepient[] GetAllRecepients();
        Beneficiary[] GetAllBeneficiaries();
        Department[] GetAllDepartments();
        Campaign[] GetAllCampaigns();
        Item[] GetAllItems();
        MainCategory[] GetAllMainCategories();
        SubCategory[] GetAllSubCategories();
        Category[] GetAllCategories();
        //Employee
        Employee[] GetEmployeesWorkingIn(Department department);
        Employee GetEmployeeManaging(Campaign campaign);
        //Campaign
        Campaign[] GetCampaginsManagedBy(Employee employee);
        Campaign[] GetCampaignsOf(Volunteer volunteer);
        Campaign[] GetCampaignsOf(Donor donor);
        Campaign[] GetCampaignsOf(Recepient recepient);
        Campaign[] GetCampaignsOf(Beneficiary beneficiary);
        //Donor
        Donor[] GetDonorsDonatingTo(Campaign campaign);
        DonorItem[] GetDonorsOf(Item item);
        DonorItem[] GetDonorsOf(Campaign campaign, Item item);
        //Volunteer
        Volunteer[] GetVolunteersOf(Campaign campaign);
        //Recepient
        Recepient[] GetRecepientsReceivingFrom(Campaign campaign);
        RecepientItem[] GetRecepientsOf(Campaign campaign, Item item);
        //Beneficiary
        Beneficiary[] GetBeneficiariesOf(Campaign campaign);
        //Item
        Item[] GetItemsReceivedBy(Recepient recepient);
        Item[] GetItemsDonatedBy(Donor donor);
        Item[] GetItemsIn(Campaign campaign);
        Item[] GetItemsOf(MainCategory mainCategory);
        Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory);
        //Department
        Department GetDepartmentOf(Employee employee);
        //Category
        SubCategory[] GetSubCategoriesOf(MainCategory mainCategory);
        //Basic Insertions
        void InsertPersons(params Person[] people);
        void InsertBeneficiary(params Beneficiary[] beneficiaries);
        void InsertDonors(params Donor[] donors);
        void InsertReceipeients(params Recepient[] recepients);
        void InsertVolunteers(params Volunteer[] volunteers);
        void InsertEmployee(params Employee[] employees);
        void InsertCampaign(params Campaign[] campaigns);
        void InsertCategories(params Category[] categories);
        void InsertDepartments(params Department[] departments);
        void InsertItems(params Item[] items);
        //Associations
        void LinkItemWithDonor(DonorItem item);
        void LinkItemWithRecepient(RecepientItem item);
        void SetCampaignManager(Campaign campaign, Employee employee);
        void RecordVolunteerParticipation(Volunteer volunteer, Campaign campaign);
        void RecordBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign);
        void SetEmployeeDepartment(Employee employee, Department department);
        void SetCategoryAsMain(Category category);
        void SetCategoryAsSub(Category category, MainCategory mainCategory);
        //Updates
        void UpdateEntity<T>(T Entity); //Tricky Tricky :D
        void UpdateLink(DonorItem donorItem, int Count);
        void UpdateLink(RecepientItem recepientItem, int Count);
        //Deletions
        void DeleteEntity<T>(T Entity); //Tricky too but easier than update :D
        void DeleteLink(DonorItem item);
        void DeleteLink(RecepientItem item);
        void FireEmployeeFromDepartment(Employee employee, Department department);
        void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign);
        void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign);
        void UnSetCategoryAsMain(MainCategory category);
        void UnSetCategoryAsSub(SubCategory category);
    }
}
