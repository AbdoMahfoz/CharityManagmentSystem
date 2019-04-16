using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using CharityManagmentSystem.Models;

namespace CharityManagmentSystem
{
    class DBConnectedMode : IDBLayer
    {
        OracleConnection conn;
        private T FillObject<T>(OracleDataReader reader)
        {
            if(typeof(T) == typeof(Person))
            {
                Person p = new Person();
                //Fill data from reader
                return (T)(object)p;
            }
            else if(typeof(T) == typeof(Campaign))
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

        public Beneficiary[] GetAllBeneficiaries()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Beneficiary";
            List<Beneficiary> benlist = new List<Beneficiary>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Beneficiary b = new Beneficiary();
                b = FillObject<Beneficiary>(dr);
                benlist.Add(b);

            }
            dr.Close();
            return benlist.ToArray();
        }
        public Campaign[] GetAllCampaigns()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Campaign";
            List<Campaign> camplist = new List<Campaign>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Campaign b = new Campaign();
                b = FillObject<Campaign>(dr);
                camplist.Add(b);

            }
            dr.Close();
            return camplist.ToArray();
        }
        public Category[] GetAllCategories()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Category";
            List<Category> catlist = new List<Category>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Category b = new Category();
                b = FillObject<Category>(dr);
                catlist.Add(b);

            }
            dr.Close();
            return catlist.ToArray();
        }
        public Department[] GetAllDepartments()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Department";
            List<Department> deplist = new List<Department>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Department b = new Department();
                b = FillObject<Department>(dr);
                deplist.Add(b);

            }
            dr.Close();
            return deplist.ToArray();
        }
        public Donor[] GetAllDonors()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Donor";
            List<Donor> donlist = new List<Donor>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Donor b = new Donor();
                b = FillObject<Donor>(dr);
                donlist.Add(b);

            }
            dr.Close();
            return donlist.ToArray();
        }
        public Employee[] GetAllEmployees()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Employee";
            List<Employee> emplist = new List<Employee>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Employee b = new Employee();
                b = FillObject<Employee>(dr);
                emplist.Add(b);

            }
            dr.Close();
            return emplist.ToArray();
        }
        public Item[] GetAllItems()
        {
            InitializeConnection();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select * from Item";
            List<Item> itemlist = new List<Item>();
            OracleDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Item b = new Item();
                b = FillObject<Item>(dr);
                itemlist.Add(b);

            }
            dr.Close();
            return itemlist.ToArray();
        }
        public MainCategory[] GetAllMainCategories()
        {
            throw new NotImplementedException();
        }
        public Person[] GetAllPersons()
        {
            throw new NotImplementedException();
        }
        public Recepient[] GetAllRecepients()
        {
            throw new NotImplementedException();
        }
        public SubCategory[] GetAllSubCategories()
        {
            throw new NotImplementedException();
        }
        public Volunteer[] GetAllVolunteers()
        {
            throw new NotImplementedException();
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public Department[] GetDepartmentsInWhich(Employee employee)
        {
            throw new NotImplementedException();
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            throw new NotImplementedException();
        }
        public Employee[] GetEmployeesManaging(Campaign campaign)
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
