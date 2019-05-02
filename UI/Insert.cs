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
using CharityManagmentSystem.Models;

namespace UI
{
    public partial class Insert : Form
    {
        CMS cms;
        Employee[] managers;
        MainCategory[] mainCategories;
        SubCategory[] subCategories;
        Type[] personTypes = { typeof(Volunteer), typeof(Beneficiary), typeof(Donor), typeof(Recepient) };
        public Insert(int i)
        {
            cms = new CMS(DataAccessMode.Connected);
            InitializeComponent();
            switch (i)
            {
                case 1:
                    InsertPerson.Visible = true;
                    break;
                case 2:
                    InsertCampaign.Visible = true;
                    break;
                case 3:
                    InsertItem.Visible = true;
                    break;
            }
        }
        private bool CheckNulls(Panel panel)
        {
            foreach (Control control in panel.Controls)
            {
                if (control is TextBox)
                {
                    if (string.IsNullOrWhiteSpace(control.Text))
                    {
                        MessageBox.Show($"Field '{control.Name.Replace("Campaign", "")}' is empty");
                        return false;
                    }
                }
                else if (control is ComboBox comboBox)
                {
                    if (comboBox.SelectedItem == null)
                    {
                        MessageBox.Show($"Please select a manager from the given list");
                        return false;
                    }
                }
            }
            return true;
        }
        private void EnableControls(Panel panel)
        {
            foreach(Control c in panel.Controls)
            {
                c.Enabled = true;
            }
        }
        private void DisableControls(Panel panel)
        {
            foreach (Control c in panel.Controls)
            {
                c.Enabled = false;
            }
        }
        private async void InsertCampaign_VisibleChanged(object sender, EventArgs e)
        {
            if (InsertCampaign.Visible)
            {
                CampaignManager.Enabled = false;
                await Task.Run(() =>
                {
                    cms.InitializeConnection();
                    managers = cms.GetAllEmployees();
                });
                foreach (var manager in managers)
                {
                    CampaignManager.Items.Add(manager.Name);
                }
                CampaignManager.Enabled = true;
            }
        }
        private async void InsertCampaignButton_Click(object sender, EventArgs e)
        {
            if (CheckNulls(InsertCampaign))
            {
                DisableControls(InsertCampaign);
                Campaign c = new Campaign()
                {
                    ID = int.Parse(CampaignID.Text),
                    Budget = long.Parse(CampaignBudget.Text),
                    Date = CampaignDate.Value,
                    Description = CampaignDescription.Text,
                    Name = CampaignName.Text,
                    Location = CampaignLocation.Text
                };
                Employee manager = managers[CampaignManager.SelectedIndex];
                await Task.Run(() =>
                {
                    cms.InsertCampaign(c);
                    cms.SetCampaignManager(c, manager);
                    cms.TerminateConnection();
                });
                EnableControls(InsertCampaign);
                Close();
            }
        }
        private async void InsertItem_VisibleChanged(object sender, EventArgs e)
        {
            if(InsertItem.Visible)
            {
                ItemMainCategory.Enabled = false;
                ItemSubCategory.Enabled = false;
                await Task.Run(() =>
                {
                    cms.InitializeConnection();
                    mainCategories = cms.GetAllMainCategories();
                    subCategories = cms.GetAllSubCategories();
                });
                ItemMainCategory.Enabled = true;
                ItemSubCategory.Enabled = true;
                foreach (var mainCategory in mainCategories)
                {
                    ItemMainCategory.Items.Add(mainCategory);
                }
                foreach(var subCategory in subCategories)
                {
                    ItemSubCategory.Items.Add(subCategory);
                }
            }
        }
        private async void InsertItemButton_Click(object sender, EventArgs e)
        {
            if(CheckNulls(InsertItem))
            {
                DisableControls(InsertItem);
                Item item = new Item()
                {
                    Name = ItemName.Text,
                    Description = ItemDescription.Text,
                    Main = mainCategories[ItemMainCategory.SelectedIndex],
                    Sub = subCategories[ItemSubCategory.SelectedIndex]
                };
                await Task.Run(() =>
                {
                    cms.InsertItems(item);
                    cms.TerminateConnection();
                });
                EnableControls(InsertItem);
                Close();
            }
        }
        private void InsertPerson_VisibleChanged(object sender, EventArgs e)
        {
            if(InsertPerson.Visible)
            {
                foreach(var type in personTypes)
                {
                    PersonType.Items.Add(type.Name);
                }
            }
        }
        private void AddLocationButton_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrWhiteSpace(PersonLocation.Text))
            {
                PersonLocationList.Items.Add(PersonLocation.Text);
                PersonLocation.Text = "";
            }
        }
        private async void InserPersonButton_Click(object sender, EventArgs e)
        {
            if(CheckNulls(InsertPerson))
            {
                DisableControls(InsertPerson);
                Person p = new Person()
                {
                    Name = PersonName.Text,
                    Mail = PersonMail.Text, 
                    Location = PersonLocationList.Items.Cast<string>().ToArray(),
                    SSN = long.Parse(PersonSSN.Text),
                };
                await Task.Run(() =>
                {
                    cms.InitializeConnection();
                    switch(PersonType.SelectedItem as string)
                    {
                        case "Volunteer":
                            cms.InsertVolunteers((Volunteer)p);
                            break;
                        case "Beneficiary":
                            cms.InsertBeneficiary((Beneficiary)p);
                            break;
                        case "Recepient":
                            cms.InsertReceipeients((Recepient)p);
                            break;
                        case "Employee":
                            cms.InsertEmployee((Employee)p);
                            break;
                    }
                });
                EnableControls(InsertPerson);
                Close();
            }
        }
    }
}
