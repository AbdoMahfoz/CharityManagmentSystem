using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charity_Managment_System.Models
{
    public class Campaign
    {
        int ID;
        DateTime Date;
        string Name;
        string Description;
        string Location;
        int Budget;
        Employee Manager;
    }
}
