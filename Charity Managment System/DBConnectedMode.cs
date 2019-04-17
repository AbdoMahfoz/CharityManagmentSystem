using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CharityManagmentSystem.Models;
using System.Data;

namespace CharityManagmentSystem
{
    class DBConnectedMode : IDBLayer
    {
        OracleConnection conn;
        private T FillObject<T>(OracleDataReader reader)
        {
            if (typeof(T) == typeof(Person))
            {
                Person p = new Person();
                //Fill data from reader
                return (T)(object)p;
            }
            else if (typeof(T) == typeof(Campaign))
            {
                Campaign c = new Campaign();
                //Fill data from reader
                return (T)(object)c;
            }
            //And so on for all of our models...
            else
            {
                throw new NotImplementedException("No implmentation available for the given type: " + typeof(T).ToString());
            }
        }
        private T[] FillList<T>(string cmdstr)
        {
            OracleCommand cmd = new OracleCommand
            {
                Connection = conn,
                CommandText = cmdstr,
                CommandType = CommandType.Text
            };
            OracleDataReader reader = cmd.ExecuteReader();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                T c = FillObject<T>(reader);
                list.Add(c);
            }
            reader.Close();
            return list.ToArray();
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            return FillList<Beneficiary>("select * from Beneficiary b, Person p where b.SSN = p.SSN");

        }
        public Campaign[] GetAllCampaigns()
        {
            return FillList<Campaign>("select * from Campaign");

        }
        public Category[] GetAllCategories()
        {
            return FillList<Category>("select * from Category");

        }
        public Department[] GetAllDepartments()
        {
            return FillList<Department>("select * from Department");

        }
        public Donor[] GetAllDonors()
        {
            return FillList<Donor>("select * from Donor d, Person p where d.SSN = p.SSN");

        }
        public Employee[] GetAllEmployees()
        {
            return FillList<Employee>("select * from Employee e, Person p where e.SSN = p.SSN");
        }
        public Item[] GetAllItems()
        {
            //return FillList<Item>("select * from Item i, MainCategory MC,SubCategory SC "+
            //  "where i.Name = MC.Name and i.Name = SC.Name");
            throw new NotImplementedException();
        }
        public MainCategory[] GetAllMainCategories()
        {
            return FillList<MainCategory>("select * from MainCategory MC ,Category C where MC.Name = C.Name");
        }
        public Person[] GetAllPersons()
        {
            return FillList<Person>("select * from Person");
        }
        public Recepient[] GetAllRecepients()
        {
            return FillList<Recepient>("select * from Recipient r,Person p where r.recipient_ssn=p.ssn");
        }
        public SubCategory[] GetAllSubCategories()
        {
            return FillList<SubCategory>("select * from SubCategory sub,category c where sub.name=c.name");
        }
        public Volunteer[] GetAllVolunteers()
        {
            return FillList<Volunteer>("select * from Volunteer");
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            return FillList<Beneficiary>("select beneficiary_ssn from Beneficiary b ,Benefit_from bf , Campaign C " +
                                         "where b.beneficiary_ssn=bf.beneficiary_ssn and bf.campaign_id=C.campaign_id");
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
        {
            return FillList<Department>("select dept_name,description from departmrnt dep , Employee emp where dep.dept_name=emp.department_name");
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
        }
        public Employee GetEmployeeManaging(Campaign campaign)
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
            conn = new OracleConnection(DBGlobals.ConnectionString);
            conn.Open();

        }
        public void TerminateConnection()
        {
            conn.Close();

        }
    }
}
