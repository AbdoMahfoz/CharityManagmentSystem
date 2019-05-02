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
    public partial class Form1 : Form
    {
        public Form1()
        {
            CMS cms = new CMS(DataAccessMode.Disconnected);
            cms.InitializeConnection();
            cms.InsertDepartments(new Department()
            {
                DeptName = "nignig",
                Description = "ding ding"
            });
            cms.TerminateConnection();
            InitializeComponent();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            new Manage(0).ShowDialog();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            new Manage(1).ShowDialog();
            Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            new Manage(2).ShowDialog();
            Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Hide();
            new Manage(3).ShowDialog();
            Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Hide();
            new Manage(4).ShowDialog();
            Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Hide();
            new Insert(2).ShowDialog();
            Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Hide();
            new Insert(1).ShowDialog();
            Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Hide();
            new Insert(3).ShowDialog();
            Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Hide();
            new CrystalReport4WF().ShowDialog();
            Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Hide();
            new crystalreport2WF().ShowDialog();
            Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Hide();
            new CrystalReport3WF().ShowDialog();
            Show();
        }
    }
}
