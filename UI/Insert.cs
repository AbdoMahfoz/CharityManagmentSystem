using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Insert : Form
    {
        public Insert(int i)
        {
            InitializeComponent();
            switch (i)
            {
                case 1:
                    panel2.Visible = true;
                    break;
                case 2:
                    panel3.Visible = true;
                    break;
                case 3:
                    panel4.Visible = true;
                    break;
               

            }
        }

        private void Insert_Load(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
