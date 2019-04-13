using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charity_Managment_System.Models
{
    public class DonorItem
    {
        Donor Donor_SSN;
        Campaign Campaign_ID;
        MainCategory ItemMainName;
        SubCategory ItemSubName;
        string Description;
        int Count;
    }
}
