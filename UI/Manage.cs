using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CharityManagmentSystem;

namespace UI
{
    public partial class Manage : Form
    {
        CMS cms;
        DataSet dataSet;
        public Manage(int i)
        {
            dataSet = new DataSet();
            cms = new CMS(DataAccessMode.Connected);
            cms.InitializeConnection();
            InitializeComponent();
            switch(i)
            {
                case 1:
                    CampaignDonorsItems.Visible = true;
                    break;
                case 2:
                    CampaignsReceipeintItems.Visible = true;
                    break;
                case 3:
                    MainSubCategory.Visible = true;
                    break;
                case 4:
                    DepartmentEmployees.Visible = true;
                    break;
                default:
                    CampaignsVolunteersBeneficiers.Visible = true;
                    break;
            }
        }
        private void CampaignDonorsItems_VisibleChanged(object sender, EventArgs e)
        {
            if (CampaignDonorsItems.Visible)
            {
                DataTable donor = cms.GetTable(@"SELECT Name_, SSN, Campaign_ID
                                                 FROM Donate_to, Donor, Person
                                                 WHERE Person.SSN = Donor.Donor_SSN AND Donate_to.Donor_SSN = Donor.Donor_SSN", TableType.CustomQuery);
                DataTable item = cms.GetTable(@"SELECT Name_, Campaign_ID
                                                FROM Donate_to, Item
                                                WHERE Item.Name_ = Donate_to.ItemName AND Item.MainName = Donate_to.ItemMainName and Item.SubName = Donate_to.ItemSubName", TableType.CustomQuery);
                dataSet.Tables.Add(cms.GetTable("Campaign"));
                dataSet.Tables.Add(donor);
                dataSet.Tables.Add(item);
                dataSet.Relations.Add(new DataRelation("CampaignDonor",
                                                       dataSet.Tables["Campaign"].Columns["ID_"], donor.Columns["Campaign_ID"]));
                dataSet.Relations.Add(new DataRelation("CampaignItem",
                                                       dataSet.Tables["Campaign"].Columns["ID_"], item.Columns["Campaign_ID"]));
                CampaignTable.DataSource = new BindingSource(dataSet, "Campaign");
                DonorsTable.DataSource = new BindingSource(CampaignTable.DataSource, "CampaignDonor");
                ItemsTable.DataSource = new BindingSource(CampaignTable.DataSource, "CampaignItem");
            }
        }
        private void MainSubCategory_VisibleChanged(object sender, EventArgs e)
        {
            if (MainSubCategory.Visible)
            {
                dataSet.Tables.Add(cms.GetTable("MainCategory"));
                dataSet.Tables.Add(cms.GetTable("SubCategory"));
                dataSet.Relations.Add(new DataRelation("MainSubCategory",
                    dataSet.Tables["MainCategory"].Columns["Name_"], dataSet.Tables["SubCategory"].Columns["Main_Name"]));
                MainCategoryTable.DataSource = new BindingSource(dataSet, "MainCategory");
                SubCategoryTable.DataSource = new BindingSource(MainCategoryTable.DataSource, "MainSubCategory");
            }
        }
        private void DepartmentEmployees_VisibleChanged(object sender, EventArgs e)
        {
            if(DepartmentEmployees.Visible)
            {
                dataSet.Tables.Add(cms.GetTable("Department"));
                dataSet.Tables.Add(cms.GetTable("Employee"));
                dataSet.Relations.Add(new DataRelation("DepartmentEmployee",
                    dataSet.Tables["Department"].Columns["DEPT_NAME"], dataSet.Tables["Employee"].Columns["Department_Name"]));
                DepartmentTable.DataSource = new BindingSource(dataSet, "Department");
                EmployeesTable.DataSource = new BindingSource(DepartmentTable.DataSource, "DepartmentEmployee");
            }
        }
        private void CampaignsReceipeintItems_VisibleChanged(object sender, EventArgs e)
        {
            if(CampaignsReceipeintItems.Visible)
            {
                DataTable recepient = cms.GetTable(@"SELECT Name_, SSN, Campaign_ID
                                                     FROM Receives_From, Recipient, Person
                                                     WHERE Person.SSN = Recipient.Recipient_SSN AND Receives_From.Recipient_SSN = Recipient.Recipient_SSN", TableType.CustomQuery);
                DataTable item = cms.GetTable(@"SELECT Name_, Campaign_ID
                                                FROM Receives_From, Item
                                                WHERE Item.Name_ = Receives_From.ItemName AND Item.MainName = Receives_From.ItemMainName and Item.SubName = Receives_From.ItemSubName", TableType.CustomQuery);
                dataSet.Tables.Add(cms.GetTable("Campaign"));
                dataSet.Tables.Add(recepient);
                dataSet.Tables.Add(item);
                dataSet.Relations.Add(new DataRelation("CampaignRecepient",
                                                       dataSet.Tables["Campaign"].Columns["ID_"], recepient.Columns["Campaign_ID"]));
                dataSet.Relations.Add(new DataRelation("CampaignItem",
                                                       dataSet.Tables["Campaign"].Columns["ID_"], item.Columns["Campaign_ID"]));
                RCampaignsTable.DataSource = new BindingSource(dataSet, "Campaign");
                RecepientsTable.DataSource = new BindingSource(RCampaignsTable.DataSource, "CampaignRecepient");
                RItemsTable.DataSource = new BindingSource(RCampaignsTable.DataSource, "CampaignItem");
            }
        }
        private void CamapignsVolunteersBeneficiers_VisibleChanged(object sender, EventArgs e)
        {
            if (CampaignsVolunteersBeneficiers.Visible)
            {
                DataTable volunteer = cms.GetTable(@"SELECT p.Name_ as Name, vi.Campaign_ID as CampaignID
                                                     FROM Person p, Volunteer v, Volunteer_in vi
                                                     WHERE p.SSN = v.Volunteer_SSN and p.SSN = vi.Volunteer_SSN", TableType.CustomQuery);
                DataTable beneficiary = cms.GetTable(@"SELECT p.Name_ as Name, rf.Campaign_ID as CampaignID
                                                       FROM Person p, Beneficiary r, Benefit_from rf
                                                       WHERE p.SSN = r.Beneficiary_SSN and p.SSN = rf.Beneficiary_SSN", TableType.CustomQuery);
                dataSet.Tables.Add(volunteer);
                dataSet.Tables.Add(beneficiary);
                dataSet.Tables.Add(cms.GetTable("Campaign"));
                dataSet.Relations.Add(new DataRelation("CampaignVolunteer", dataSet.Tables["Campaign"].Columns["ID_"], volunteer.Columns["CampaignID"]));
                dataSet.Relations.Add(new DataRelation("CampaignBeneficiary", dataSet.Tables["Campaign"].Columns["ID_"], beneficiary.Columns["CampaignID"]));
                VCampaignsTable.DataSource = new BindingSource(dataSet, "Campaign");
                VolunteersTable.DataSource = new BindingSource(VCampaignsTable.DataSource, "CampaignVolunteer");
                BeneficiersTable.DataSource = new BindingSource(VCampaignsTable.DataSource, "CampaignBeneficiary");
            }
        }
        private void Manage_FormClosing(object sender, FormClosingEventArgs e)
        {
            cms.TerminateConnection();
        }
    }
}
