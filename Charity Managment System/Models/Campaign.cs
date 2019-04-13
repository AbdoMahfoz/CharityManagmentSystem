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
        Employee Manager_SSN;
        List<Employee> Employee_SSN;
        List<Volunteer> Volunteer_SSN;
        List<Beneficiary> Beneficiary_SSN;
        List<Recepient> Recepient_SSN;
        List<Donor> Donor_SSN;
        
    }
}
