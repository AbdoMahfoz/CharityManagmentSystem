Create Table Person(
	SSN NUMBER(10) Primary key, 
	Name_ VarChar2(20), 
	Mail VarChar2(20) Unique
);

Create Table Person_Location(
	Person_SSN Number(10) references Person,
	Location_No NUMBER(10),
	constraint PersonLocation primary key (Person_SSN, Location_No)
);

Create Table Volunteer(
	Volunteer_SSN NUMBER(10) Primary key references Person
);

Create Table Department(
	Dept_Name VarChar2(20) Primary key,
	Description VarChar2(150)
);

Create Table Employee(
	Employee_SSN Number(10) Primary key references Person, 
	Department_Name VarChar2(20) references Department, 
	Salary Number(20)
);

Create Table Campaign(
	ID_ NUMBER(10) Primary key, 
	Employee_SSN Number(10) references Person, 
	Date_ Date Not null, 
	Name_ VarChar2(20), 
	Description_ VarChar2(150), 
	Location_ VarChar2(20), 
	Budget Number(30)
);

Create Table Volunteer_in(
	Volunteer_SSN NUMBER(10) references Person(SSN),
	Campaign_ID NUMBER(10) references Campaign(ID_),
	constraint VolunteerIn primary key (Volunteer_SSN,Campaign_ID)
);

Create Table Beneficiary(
	Beneficiary_SSN Number(10) primary key references Person(SSN)
);

Create Table Benefit_from(
	Beneficiary_SSN Number(10) references Beneficiary(Beneficiary_SSN),
	Campaign_ID NUMBER(10) references Campaign(ID_),
	constraint Benefit_from primary key (Beneficiary_SSN,Campaign_ID)
);

Create Table Donor(
	Donor_SSN Number(10) Primary key references Person(SSN)
);

Create Table Recipient(
	Recipient_SSN Number(10) Primary key references Person(SSN)
);

Create Table Category_(
	Name_ VarChar(20) Primary key,
	Description_ VarChar2(150)
);

Create Table MainCategory(
	Name_ VarChar2(20) Primary key references Category_(Name_)
);

Create Table SubCategory(
	Name_ VarChar2(20) Primary key references Category_(Name_),
	Main_Name VarChar2(20) references MainCategory(Name_)
);

Create Table Item(
	Name_ VarChar2(20) unique not null,
	MainName VarChar2(20) references MainCategory(Name_), 
	Description_ VarChar2(150), 
	SubName VarChar2(20) references SubCategory(Name_), 
	constraint ItemPK Primary key (Name_, MainName, SubName)
);

Create Table Donate_to(
	Donor_SSN Number(10) references Donor(Donor_SSN),
	Campaign_ID Number(10) references Campaign(ID_),
	ItemMainName VarChar2(20),
	Description_ VarChar2(150),
	ItemSubName VarChar2(20),
	Count_ Number(10),
	constraint DonatePK Primary key (Donor_SSN, Campaign_ID, ItemMainName, ItemSubName),
	constraint DonateFK Foreign key (ItemMainName, ItemSubName) REFERENCES Item(MainName, SubName)
);

Create Table Receives_From(
	Recipient_SSN Number(10) references Recipient(Recipient_SSN), 
	Campaign_ID Number(10) references Campaign(ID_), 
	MainName VarChar2(20), 
	Description_ VarChar2(150), 
	SubName VarChar2(20), 
	Count_ Number(10),
	constraint ReceivesFrom primary key (Recipient_SSN, Campaign_ID, MainName, SubName), 
	constraint ReceiveFromFK Foreign key (MainName, SubName) REFERENCES Item(MainName, SubName)
);