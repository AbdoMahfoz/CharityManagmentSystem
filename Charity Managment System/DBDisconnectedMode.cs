using System;
using System.Collections.Generic;
using System.Linq;
using Oracle.DataAccess.Client;
using CharityManagmentSystem.Models;
using System.Data;
using System.Threading;

namespace CharityManagmentSystem
{
    /// <summary>
    /// Downloads and saves tables into memory, mainuplates them in memory then uploads changes back to database
    /// </summary>
    class DBDisconnectedMode : IDBLayer
    {
        DataSet dataSet;
        readonly Dictionary<string, OracleDataAdapter> adapters;
        public DBDisconnectedMode()
        {
            adapters = new Dictionary<string, OracleDataAdapter>();
        }
        /// <summary>
        /// Flush exisiting dataSet and make a new one
        /// </summary>
        public void InitializeConnection()
        {
            TerminateConnection();
            dataSet = new DataSet();
        }
        /// <summary>
        /// Flush exisiting dataSet then clear it and the adapters
        /// </summary>
        public void TerminateConnection()
        {
            foreach(var adapter in adapters)
            {
                new OracleCommandBuilder(adapter.Value);
                adapter.Value.Update(dataSet, adapter.Key);
            }
            adapters.Clear();
            dataSet = null;
        }
        /// <summary>
        /// Check if the dataSet has the specificed table. If not, download it.
        /// </summary>
        /// <param name="tableName">The name of the table to be checked</param>
        public void FetchTable(string tableName)
        {
            if(!adapters.ContainsKey(tableName))
            {
                var adapter = new OracleDataAdapter($"SELECT * FROM {tableName}", DBGlobals.ConnectionString);
                DataSet tmp = new DataSet();
                adapter.Fill(tmp, tableName);
                lock(dataSet)
                {
                    dataSet.Merge(tmp);
                    dataSet.AcceptChanges();
                }
                lock(adapters)
                {
                    adapters.Add(tableName, adapter);
                }
            }
        }
        /// <summary>
        /// Fetches all given tables concurrently
        /// </summary>
        /// <param name="actions">Table names</param>
        void ParallelFetch(params string[] tables)
        {
            Thread[] threads = new Thread[tables.Length];
            for(int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart((object o) =>
                {
                    int j = (int)o;
                    FetchTable(tables[j]);
                }));
                threads[i].Start(i);
            }
            foreach(Thread t in threads)
            {
                t.Join();
            }
        }
        /// <summary>
        /// Automatically creates an object of type <typeparamref name="T"/> and fills it with releveant data from the given rows
        /// </summary>
        /// <typeparam name="T">The type of the object to be created</typeparam>
        /// <param name="rows">The rows that contain the object's data</param>
        T QuerySelect<T>(params DataRow[] rows) where T : new()
        {
            if (typeof(T) == typeof(Item))
            {
                return (T)(object)new Item()
                {
                    Name = rows[0].Field<string>("Name_"),
                    Description = rows[0].Field<string>("Description_"),
                    Main = (from entry3 in dataSet.Tables["MainCategory"].AsEnumerable()
                            join entry4 in dataSet.Tables["Category_"].AsEnumerable()
                            on entry3.Field<string>("Name_") equals entry4.Field<string>("Name_")
                            where entry3.Field<string>("Name_") == rows[0].Field<string>("MainName")
                            select QuerySelect<MainCategory>(entry3)).SingleOrDefault(),
                    Sub = (from entry3 in dataSet.Tables["SubCategory"].AsEnumerable()
                           join entry4 in dataSet.Tables["Category_"].AsEnumerable()
                           on entry3.Field<string>("Name_") equals entry4.Field<string>("Name_")
                           where entry3.Field<string>("Name_") == rows[0].Field<string>("SubName")
                           select QuerySelect<SubCategory>(entry3)).SingleOrDefault(),
                };
            }
            else
            {
                var list = (from x in rows select (from e in x.Table.Columns.Cast<DataColumn>() select e.ColumnName).ToArray()).ToArray();
                T res = new T();
                foreach (var Property in typeof(T).GetFields())
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        string x = Array.Find(list[i], new Predicate<string>(
                            (string s) => s.Substring(s.LastIndexOf('.') + 1).Replace("_", "") == Property.Name));
                        if (!string.IsNullOrEmpty(x))
                        {
                            Property.SetValue(res, rows[i][x]);
                        }
                    }
                }
                return res;
            }
        }
        /// <summary>
        /// Gets the DataRows that represent the given entites.
        /// It fills DataRows with any field in the given entity that matches a column name in the DataRow.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="Entities">Entite(s) to be inserted</param>
        DataRow[] ToDataRow<T>(params T[] Entities)
        {
            Dictionary<string, System.Reflection.FieldInfo> Fields = new Dictionary<string, System.Reflection.FieldInfo>();
            foreach (var field in typeof(T).GetFields())
            {
                Fields.Add(field.Name, field);
            }
            string tableName = typeof(T).Name;
            if(tableName == "Category")
            {
                tableName += "_";
            }
            List<DataRow> res = new List<DataRow>();
            foreach (var Entity in Entities)
            {
                DataRow row = dataSet.Tables[tableName].NewRow();
                foreach (DataColumn column in row.Table.Columns)
                {
                    if (Fields.TryGetValue(column.ColumnName.Replace("_", ""), out var value))
                    {
                        row[column.ColumnName] = value.GetValue(Entity);
                    }
                }
                res.Add(row);
            }
            return res.ToArray();
        }
        /// <summary>
        /// Gets the DataRow that represent the given entity.
        /// It fills the DataRow with any field in the given entity that matches a column name in the DataRow.
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="Entities">Entite(s) to be inserted</param>
        DataRow ToDataRow<T>(T Entity)
        {
            return ToDataRow(new T[] { Entity })[0];
        }
        /// <summary>
        /// Inserts the derivatives of person properly in the dataSet
        /// </summary>
        /// <param name="people">The derivatives to be inserted</param>
        void PersonDerivativeInserter(params Person[] people)
        {
            string tableName = people[0].GetType().Name;
            string SSNname = tableName + "_SSN";
            ParallelFetch("Person", tableName);
            dataSet.Tables["Person"].Rows.Add(ToDataRow(people));
            foreach (var person in people)
            {
                DataRow row = dataSet.Tables[tableName].NewRow();
                row[SSNname] = person.SSN;
                if (person is Employee e)
                {
                    row["Salary"] = e.Salary;
                }
                if(person.GetType() == typeof(Person))
                {
                    throw new Exception("PersonDerivativeInserter should receive an object of Person." +
                                        "\nThis function only serves derivatives of person." +
                                        "\nIf you want to insert a person, try using ToDataRow");
                }
                dataSet.Tables[tableName].Rows.Add(row);
            }
            dataSet.Tables["Person"].AcceptChanges();
            dataSet.Tables[tableName].AcceptChanges();
        }
        public Beneficiary[] GetAllBeneficiaries()
        {
            ParallelFetch("Beneficiary", "Person");
            var res = from entry in dataSet.Tables["Beneficiary"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry2.Field<int>("SSN")
                      select QuerySelect<Beneficiary>(entry2);
            return res.ToArray();
        }
        public Campaign[] GetAllCampaigns()
        {
            FetchTable("Campaign");
            var res = from entry in dataSet.Tables["Campaign"].AsEnumerable()
                      select QuerySelect<Campaign>(entry);
            return res.ToArray();
        }
        public Category[] GetAllCategories()
        {
            FetchTable("Category_");
            var res = from entry in dataSet.Tables["Category_"].AsEnumerable()
                      select QuerySelect<Category>(entry);
            return res.ToArray();
        }
        public Department[] GetAllDepartments()
        {
            FetchTable("Department");
            var res = from entry in dataSet.Tables["Department"].AsEnumerable()
                      select QuerySelect<Department>(entry);
            return res.ToArray();
        }
        public Donor[] GetAllDonors()
        {
            ParallelFetch("Donor", "Person");
            var res = from entry in dataSet.Tables["Donor"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry2.Field<int>("SSN")
                      select QuerySelect<Donor>(entry2);
            return res.ToArray();
        }
        public Employee[] GetAllEmployees()
        {
            ParallelFetch("Employee", "Person");
            var res = from entry in dataSet.Tables["Employee"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Employee_SSN") equals entry2.Field<int>("SSN")
                      select QuerySelect<Employee>(entry, entry2);
            return res.ToArray();
        }
        public Item[] GetAllItems()
        {
            ParallelFetch("Item", "MainCategory", "SubCategory", "Category_");
            var res = from entry in dataSet.Tables["Item"].AsEnumerable()
                      select QuerySelect<Item>(entry);
            return res.ToArray();
        }
        public MainCategory[] GetAllMainCategories()
        {
            ParallelFetch("MainCategory", "Category_");
            var res = from entry in dataSet.Tables["MainCategory"].AsEnumerable()
                      join entry2 in dataSet.Tables["Category_"].AsEnumerable()
                      on entry.Field<string>("Name_") equals entry2.Field<string>("Name_")
                      select QuerySelect<MainCategory>(entry);
            return res.ToArray();
        }
        public Person[] GetAllPersons()
        {
            FetchTable("Person");
            var res = from entry in dataSet.Tables["Person"].AsEnumerable()
                      select QuerySelect<Person>(entry);
            return res.ToArray();
        }
        public Recepient[] GetAllRecepients()
        {
            ParallelFetch("Recepient", "Person");
            var res = from entry in dataSet.Tables["Recepient"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Recepient_SSN") equals entry2.Field<int>("SSN")
                      select QuerySelect<Recepient>(entry2);
            return res.ToArray();
        }
        public SubCategory[] GetAllSubCategories()
        {
            ParallelFetch("SubCategory", "Category_");
            var res = from entry in dataSet.Tables["SubCategory"].AsEnumerable()
                      join entry2 in dataSet.Tables["Category_"].AsEnumerable()
                      on entry.Field<string>("Name_") equals entry2.Field<string>("Name_")
                      select QuerySelect<SubCategory>(entry2);
            return res.ToArray();
        }
        public Volunteer[] GetAllVolunteers()
        {
            ParallelFetch("Volunteer", "Person");
            var res = from entry in dataSet.Tables["Volunteer"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Volunteer_SSN") equals entry2.Field<int>("SSN")
                      select QuerySelect<Volunteer>(entry2);
            return res.ToArray();
        }
        public Beneficiary[] GetBeneficiariesOf(Campaign campaign)
        {
            ParallelFetch("Beneficiary", "Person", "Benefit_From");
            var res = from entry in dataSet.Tables["Beneficiary"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry2.Field<int>("SSN")
                      join entry3 in dataSet.Tables["Benefit_From"].AsEnumerable()
                      on entry.Field<int>("Beneficiary_SSN") equals entry3.Field<int>("Beneficiary_SSN")
                      where entry3.Field<int>("Campaign_ID") == campaign.ID
                      select QuerySelect<Beneficiary>(entry2);
            return res.ToArray();
        }
        public Department GetDepartmentOf(Employee employee)
        {
            ParallelFetch("Employee", "Department");
            var res = from entry in dataSet.Tables["Employee"].AsEnumerable()
                      join entry2 in dataSet.Tables["Department"].AsEnumerable()
                      on entry.Field<string>("Department_Name") equals entry2.Field<string>("Dept_Name")
                      where entry.Field<int>("Employee_SSN") == employee.SSN
                      select QuerySelect<Department>(entry2);
            return res.Single();
        }
        public Employee[] GetEmployeesWorkingIn(Department department)
        {
            ParallelFetch("Employee", "Person", "Department");
            var res = from entry in dataSet.Tables["Employee"].AsEnumerable()
                      join entry2 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Employee_SSN") equals entry2.Field<int>("SSN")
                      where entry.Field<string>("Department_Name") == department.DeptName
                      select QuerySelect<Employee>(entry, entry2);
            return res.ToArray();
        }
        public Employee GetEmployeeManaging(Campaign campaign)
        {
            ParallelFetch("Employee", "Person", "Campaign");
            var res = from entry in dataSet.Tables["Campaign"].AsEnumerable()
                      join entry2 in dataSet.Tables["Employee"].AsEnumerable()
                      on entry.Field<int>("Employee_SSN") equals entry2.Field<int>("Employee_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry2.Field<int>("Employee_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("ID_") == campaign.ID
                      select QuerySelect<Employee>(entry2, entry3);
            return res.SingleOrDefault();
        }
        public Donor[] GetDonorsDonatingTo(Campaign campaign)
        {
            ParallelFetch("Donate_To", "Donor", "Person");
            var res = from entry in dataSet.Tables["Donate_To"].AsEnumerable()
                      join entry2 in dataSet.Tables["Donor"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry2.Field<int>("Donor_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("Campaign_ID") == campaign.ID
                      select QuerySelect<Donor>(entry2, entry3);
            return res.ToArray();
        }
        public DonorItem[] GetDonorsOf(Campaign campaign, Item item)
        {
            ParallelFetch("Donate_To", "Donor", "Person");
            var res = from entry in dataSet.Tables["Donate_To"].AsEnumerable()
                      join entry2 in dataSet.Tables["Donor"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry2.Field<int>("Donor_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("Campaign_ID") == campaign.ID && 
                            entry.Field<string>("ItemName") == item.Name &&
                            entry.Field<string>("ItemMainName") == item.Main.Name &&
                            entry.Field<string>("ItemSubName") == item.Sub.Name
                      select new DonorItem()
                      {
                          Donor = QuerySelect<Donor>(entry2, entry3),
                          Campaign = campaign,
                          Item = item,
                          Count = entry.Field<int>("Count_")
                      };
            return res.ToArray();
        }
        public Volunteer[] GetVolunteersOf(Campaign campaign)
        {
            ParallelFetch("Volunteer_In", "Volunteer", "Person");
            var res = from entry in dataSet.Tables["Volunteer_In"].AsEnumerable()
                      join entry2 in dataSet.Tables["Volunteer"].AsEnumerable()
                      on entry.Field<int>("Volunteer_SSN") equals entry2.Field<int>("Volunteer_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry2.Field<int>("Volunteer_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("Campaign_ID") == campaign.ID
                      select QuerySelect<Volunteer>(entry2, entry3);
            return res.ToArray();
        }
        public Recepient[] GetRecepientsReceivingFrom(Campaign campaign)
        {
            var res = from entry in dataSet.Tables["Receives_From"].AsEnumerable()
                      join entry2 in dataSet.Tables["Recepient"].AsEnumerable()
                      on entry.Field<int>("Recepient_SSN") equals entry2.Field<int>("Recepient_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry2.Field<int>("Recepient_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("Campaign_ID") == campaign.ID
                      select QuerySelect<Recepient>(entry2, entry3);
            return res.ToArray();
        }
        public RecepientItem[] GetRecepientsOf(Campaign campaign, Item item)
        {
            ParallelFetch("Receives_From", "Recepient", "Person");
            var res = from entry in dataSet.Tables["Receives_From"].AsEnumerable()
                      join entry2 in dataSet.Tables["Recepient"].AsEnumerable()
                      on entry.Field<int>("Recepient_SSN") equals entry2.Field<int>("Recepient_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry2.Field<int>("Recepient_SSN") equals entry3.Field<int>("SSN")
                      where entry.Field<int>("Campaign_ID") == campaign.ID &&
                            entry.Field<string>("ItemName") == item.Name &&
                            entry.Field<string>("ItemMainName") == item.Main.Name &&
                            entry.Field<string>("ItemSubName") == item.Sub.Name
                      select new RecepientItem()
                      {
                          Campaign = campaign,
                          Item = item,
                          Recipient = QuerySelect<Recepient>(entry2, entry3)
                      };
            return res.ToArray();
        }
        public Item[] GetItemsReceivedBy(Recepient recepient)
        {
            ParallelFetch("Receives_From", "Item");
            var res = from entry in dataSet.Tables["Receives_From"].AsEnumerable()
                      join entry2 in dataSet.Tables["Item"].AsEnumerable()
                      on new
                      {
                          Name = entry.Field<string>("ItemName"),
                          MainName = entry.Field<string>("ItemMainName"),
                          SubName = entry.Field<string>("ItemSubName")
                      }
                      equals new
                      {
                          Name = entry2.Field<string>("Name_"),
                          MainName = entry2.Field<string>("MainName"),
                          SubName = entry2.Field<string>("SubName")
                      }
                      select QuerySelect<Item>(entry2);
            return res.ToArray();
        }
        public Item[] GetItemsDonatedBy(Donor donor)
        {
            ParallelFetch("Donate_to", "Item");
            var res = from entry in dataSet.Tables["Donate_to"].AsEnumerable()
                      join entry2 in dataSet.Tables["Item"].AsEnumerable()
                      on new
                      {
                          Name = entry.Field<string>("ItemName"),
                          MainName = entry.Field<string>("ItemMainName"),
                          SubName = entry.Field<string>("ItemSubName")
                      }
                      equals new
                      {
                          Name = entry2.Field<string>("Name_"),
                          MainName = entry2.Field<string>("MainName"),
                          SubName = entry2.Field<string>("SubName")
                      }
                      where entry.Field<int>("Donor_SSN") == donor.SSN
                      select QuerySelect<Item>(entry2);
            return res.ToArray();
        }
        public Item[] GetItemsIn(Campaign campaign)
        {
            ParallelFetch("Donate_to", "Item");
            var res = from entry in dataSet.Tables["Donate_to"].AsEnumerable()
                      join entry2 in dataSet.Tables["Item"].AsEnumerable()
                      on new
                      {
                          Name = entry.Field<string>("ItemName"),
                          MainName = entry.Field<string>("ItemMainName"),
                          SubName = entry.Field<string>("ItemSubName")
                      }
                      equals new
                      {
                          Name = entry2.Field<string>("Name_"),
                          MainName = entry2.Field<string>("MainName"),
                          SubName = entry2.Field<string>("SubName")
                      }
                      where entry.Field<int>("Campaign_ID") == campaign.ID
                      select QuerySelect<Item>(entry2);
            return res.ToArray();
        }
        public Item[] GetItemsOf(MainCategory mainCategory)
        {
            FetchTable("Item");
            var res = from entry in dataSet.Tables["Item"].AsEnumerable()
                      where entry.Field<string>("MainName") == mainCategory.Name
                      select QuerySelect<Item>(entry);
            return res.ToArray();
        }
        public Item[] GetItemsOf(MainCategory mainCategory, SubCategory subCategory)
        {
            FetchTable("Item");
            var res = from entry in dataSet.Tables["Item"].AsEnumerable()
                      where entry.Field<string>("MainName") == mainCategory.Name &&
                            entry.Field<string>("SubName") == subCategory.Name
                      select QuerySelect<Item>(entry);
            return res.ToArray();
        }
        public Campaign[] GetCampaginsManagedBy(Employee employee)
        {
            FetchTable("Campaign");
            var res = from entry in dataSet.Tables["Campaign"].AsEnumerable()
                      where entry.Field<int>("Employee_SSN") == employee.SSN
                      select QuerySelect<Campaign>(entry);
            return res.ToArray();
        }
        public Campaign[] GetCampaignsOf(Volunteer volunteer)
        {
            ParallelFetch("Volunteer_in", "Campaign");
            var res = from entry in dataSet.Tables["Volunteer_in"].AsEnumerable()
                      join entry2 in dataSet.Tables["Campaign"].AsEnumerable()
                      on entry.Field<int>("Campaign_ID") equals entry2.Field<int>("ID_")
                      where entry.Field<int>("Volunteer_SSN") == volunteer.SSN
                      select QuerySelect<Campaign>(entry2);
            return res.ToArray();
        }
        public Campaign[] GetCampaignsOf(Donor donor)
        {
            ParallelFetch("Donate_to", "Campaign");
            var res = from entry in dataSet.Tables["Donate_to"].AsEnumerable()
                      join entry2 in dataSet.Tables["Campaign"].AsEnumerable()
                      on entry.Field<int>("Campaign_ID") equals entry2.Field<int>("ID_")
                      where entry.Field<int>("Donor_SSN") == donor.SSN
                      select QuerySelect<Campaign>(entry2);
            return res.ToArray();
        }
        public Campaign[] GetCampaignsOf(Recepient recepient)
        {
            ParallelFetch("Receives_From", "Campaign");
            var res = from entry in dataSet.Tables["Receives_From"].AsEnumerable()
                      join entry2 in dataSet.Tables["Campaign"].AsEnumerable()
                      on entry.Field<int>("Campaign_ID") equals entry2.Field<int>("ID_")
                      where entry.Field<int>("Recepient_SSN") == recepient.SSN
                      select QuerySelect<Campaign>(entry2);
            return res.ToArray();
        }
        public Campaign[] GetCampaignsOf(Beneficiary beneficiary)
        {
            ParallelFetch("Benefit_from", "Campaign");
            var res = from entry in dataSet.Tables["Benefit_from"].AsEnumerable()
                      join entry2 in dataSet.Tables["Campaign"].AsEnumerable()
                      on entry.Field<int>("Campaign_ID") equals entry2.Field<int>("ID_")
                      where entry.Field<int>("Beneficiary_SSN") == beneficiary.SSN
                      select QuerySelect<Campaign>(entry2);
            return res.ToArray();
        }
        public DonorItem[] GetDonorsOf(Item item)
        {
            ParallelFetch("Donate_to", "Donor", "Person", "Campaign");
            var res = from entry in dataSet.Tables["Donate_to"].AsEnumerable()
                      join entry2 in dataSet.Tables["Donor"].AsEnumerable()
                      on entry.Field<int>("Donor_SSN") equals entry2.Field<int>("Donor_SSN")
                      join entry3 in dataSet.Tables["Person"].AsEnumerable()
                      on entry2.Field<int>("Donor_SSN") equals entry3.Field<int>("SSN")
                      join entry4 in dataSet.Tables["Campaign"].AsEnumerable()
                      on entry.Field<int>("Campaign_ID") equals entry4.Field<int>("ID_")
                      where entry.Field<string>("ItemName") == item.Name &&
                            entry.Field<string>("ItemMainName") == item.Main.Name &&
                            entry.Field<string>("ItemSubName") == item.Sub.Name
                      select new DonorItem()
                      {
                          Donor = QuerySelect<Donor>(entry2, entry3),
                          Item = item,
                          Campaign = QuerySelect<Campaign>(entry4),
                          Count = entry.Field<int>("Count_")
                      };
            return res.ToArray();
        }
        public SubCategory[] GetSubCategoriesOf(MainCategory mainCategory)
        {
            ParallelFetch("MainCategory", "Category_");
            var res = from entry in dataSet.Tables["SubCategory"].AsEnumerable()
                      join entry2 in dataSet.Tables["Category_"].AsEnumerable()
                      on entry.Field<string>("Name_") equals entry2.Field<string>("Name_")
                      where entry.Field<string>("Main_Name") == mainCategory.Name
                      select QuerySelect<SubCategory>(entry, entry2);
            return res.ToArray();
        }
        public void InsertPersons(params Person[] people)
        {
            FetchTable("Person");
            dataSet.Tables["Person"].Rows.Add(ToDataRow(people));
            dataSet.Tables["Person"].AcceptChanges();
        }
        public void InsertBeneficiary(params Beneficiary[] beneficiaries)
        {
            PersonDerivativeInserter(beneficiaries);
        }
        public void InsertDonors(params Donor[] donors)
        {
            PersonDerivativeInserter(donors);
        }
        public void InsertReceipeients(params Recepient[] recepients)
        {
            PersonDerivativeInserter(recepients);
        }
        public void InsertVolunteers(params Volunteer[] volunteers)
        {
            PersonDerivativeInserter(volunteers);
        }
        public void InsertEmployee(params Employee[] employees)
        {
            PersonDerivativeInserter(employees);
        }
        public void InsertCampaign(params Campaign[] campaigns)
        {
            FetchTable("Campaign");
            foreach(var row in ToDataRow(campaigns))
            {
                dataSet.Tables["Campaign"].Rows.Add(row);
            }
            dataSet.Tables["Campaign"].AcceptChanges();
        }
        public void InsertCategories(params Category[] categories)
        {
            FetchTable("Category_");
            foreach(var row in ToDataRow(categories))
            {
                dataSet.Tables["Category_"].Rows.Add(row);
            }
            dataSet.Tables["Category_"].AcceptChanges();
        }
        public void InsertDepartments(params Department[] departments)
        {
            FetchTable("Department");
            foreach(var row in ToDataRow(departments))
            {
                dataSet.Tables["Department"].Rows.Add(row);
            }
            dataSet.Tables["Department"].AcceptChanges();
        }
        public void InsertItems(params Item[] items)
        {
            FetchTable("Item");
            foreach(var item in items)
            {
                DataRow row = dataSet.Tables["Item"].NewRow();
                row["Name_"] = item.Name;
                row["Description_"] = item.Description;
                row["MainName"] = item.Main.Name;
                row["SubName"] = item.Sub.Name;
                dataSet.Tables["Item"].Rows.Add(row);
            }
            dataSet.Tables["Item"].AcceptChanges();
        }
        public void LinkItemWithDonor(DonorItem item)
        {
            FetchTable("Donate_to");
            DataRow row = dataSet.Tables["Donate_to"].NewRow();
            row["ItemName"] = item.Item.Name;
            row["ItemMainName"] = item.Item.Main.Name;
            row["ItemSubName"] = item.Item.Sub.Name;
            row["Donor_SSN"] = item.Donor.SSN;
            row["Campaign_ID"] = item.Campaign.ID;
            row["Count_"] = item.Count;
            dataSet.Tables["Donate_to"].Rows.Add(row);
            dataSet.Tables["Donate_to"].AcceptChanges();
        }
        public void LinkItemWithRecepient(RecepientItem item)
        {
            FetchTable("Receieves_From");
            DataRow row = dataSet.Tables["Receieves_From"].NewRow();
            row["ItemName"] = item.Item.Name;
            row["ItemMainName"] = item.Item.Main.Name;
            row["ItemSubName"] = item.Item.Sub.Name;
            row["Recepient_SSN"] = item.Recipient.SSN;
            row["Campaign_ID"] = item.Campaign.ID;
            row["Count_"] = item.Count;
            dataSet.Tables["Receieves_From"].Rows.Add(row);
            dataSet.Tables["Receieves_From"].AcceptChanges();
        }
        public void SetCampaignManager(Campaign campaign, Employee employee)
        {
            FetchTable("Campaign");
            DataRow row = (from entry in dataSet.Tables["Campaign"].AsEnumerable()
                           where entry.Field<int>("ID_") == campaign.ID
                           select entry).Single();
            row["Employe_SSN"] = employee.SSN;
            dataSet.Tables["Campaign"].AcceptChanges();
        }
        public void RecordVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            FetchTable("Volunteer_in");
            DataRow row = dataSet.Tables["Volunteer_in"].NewRow();
            row["Volunteer_SSN"] = volunteer.SSN;
            row["Campaign_ID"] = campaign.ID;
            dataSet.Tables["Volunteer_in"].Rows.Add(row);
            dataSet.Tables["Volunteer_in"].AcceptChanges();
        }
        public void RecordBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            FetchTable("Benefit_from");
            DataRow row = dataSet.Tables["Benefit_from"].NewRow();
            row["Beneficiary_SSN"] = beneficiary.SSN;
            row["Campaign_ID"] = campaign.ID;
            dataSet.Tables["Benefit_from"].Rows.Add(row);
            dataSet.Tables["Benefit_from"].AcceptChanges();
        }
        public void SetEmployeeDepartment(Employee employee, Department department)
        {
            FetchTable("Employee");
            var row = (from entry in dataSet.Tables["Employee"].AsEnumerable()
                       where entry.Field<int>("Employee_SSN") == employee.SSN
                       select entry).Single();
            row["Department_Name"] = department.DeptName;
            dataSet.Tables["Employee"].AcceptChanges();
        }
        public void SetCategoryAsMain(Category category)
        {
            FetchTable("MainCategory");
            dataSet.Tables["MainCategory"].Rows.Add(ToDataRow(category));
            dataSet.Tables["MainCategory"].AcceptChanges();
        }
        public void SetCategoryAsSub(Category category, MainCategory mainCategory)
        {
            ParallelFetch("SubCategory", "Category_");
            DataRow row = ToDataRow(category);
            row["Main_Name"] = mainCategory.Name;
            dataSet.Tables["SubCategory"].Rows.Add(row);
            dataSet.Tables["SubCategory"].AcceptChanges();
        }
        public void UpdateEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void UpdateLink(DonorItem donorItem)
        {
            FetchTable("Donate_to");
            var row = (from entry in dataSet.Tables["Donate_to"].AsEnumerable()
                       where entry.Field<int>("Campaign_ID") == donorItem.Campaign.ID &&
                             entry.Field<int>("Donor_SSN") == donorItem.Donor.SSN &&
                             entry.Field<string>("ItemName") == donorItem.Item.Name &&
                             entry.Field<string>("ItemMainName") == donorItem.Item.Main.Name &&
                             entry.Field<string>("ItemSubName") == donorItem.Item.Sub.Name
                       select entry).Single();
            row["Count_"] = donorItem.Count;
            dataSet.Tables["Donate_to"].AcceptChanges();
        }
        public void UpdateLink(RecepientItem recepientItem)
        {
            FetchTable("Receives_From");
            var row = (from entry in dataSet.Tables["Receives_From"].AsEnumerable()
                       where entry.Field<int>("Campaign_ID") == recepientItem.Campaign.ID &&
                             entry.Field<int>("Recepient_SSN") == recepientItem.Recipient.SSN &&
                             entry.Field<string>("ItemName") == recepientItem.Item.Name &&
                             entry.Field<string>("ItemMainName") == recepientItem.Item.Main.Name &&
                             entry.Field<string>("ItemSubName") == recepientItem.Item.Sub.Name
                       select entry).Single();
            row["Count_"] = recepientItem.Count;
            dataSet.Tables["Receives_From"].AcceptChanges();
        }
        public void DeleteEntity<T>(T Entity)
        {
            throw new NotImplementedException();
        }
        public void DeleteLink(DonorItem item)
        {
            throw new NotImplementedException();
        }
        public void DeleteLink(RecepientItem item)
        {
            throw new NotImplementedException();
        }
        public void EraseVolunteerParticipation(Volunteer volunteer, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void EraseBeneficiaryParticipation(Beneficiary beneficiary, Campaign campaign)
        {
            throw new NotImplementedException();
        }
        public void UnSetCategoryAsMain(MainCategory category)
        {
            throw new NotImplementedException();
        }
        public void UnSetCategoryAsSub(SubCategory category)
        {
            throw new NotImplementedException();
        }
    }
}
