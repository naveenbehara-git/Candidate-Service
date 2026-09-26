GO

IF OBJECT_ID('dbo.Candidate', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Candidate]
    (
        CandidateId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Candidate PRIMARY KEY,

        UserId UNIQUEIDENTIFIER NOT NULL,

        FirstName NVARCHAR(100) NOT NULL,

        LastName NVARCHAR(100) NOT NULL,

        Email NVARCHAR(255) NOT NULL CONSTRAINT UQ_Candidate_Email UNIQUE,

        PhoneNumber NVARCHAR(20) NULL,

        Gender INT NOT NULL,

        DOB DATETIME2 NULL,

        MaritalStatus VARCHAR(15) NULL,

        Certifications NVARCHAR(MAX) NULL,

        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Candidate_CreatedAt DEFAULT SYSUTCDATETIME(),

        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Candidate_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT CK_Candidate_MaritalStatus
        CHECK (MaritalStatus IN ('Single','Married','Divorced','Widowed') OR MaritalStatus IS NULL)
    );
    PRINT 'Table [dbo].[Candidate] created successfully.';
END
GO

IF OBJECT_ID('dbo.Address', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.[Address]
    (
        Id INT IDENTITY(1,1) CONSTRAINT PK_Address PRIMARY KEY,

        CandidateId INT NOT NULL,

        Address1 NVARCHAR(255) NOT NULL,

        Address2 NVARCHAR(255) NULL,

        AddressType INT NOT NULL,

        City NVARCHAR(100) NULL,

        [State] NVARCHAR(100) NULL,

        Country NVARCHAR(100) NULL,

        PostalCode INT NULL,
            
        CONSTRAINT FK_Address_Candidate
            FOREIGN KEY (CandidateId)
            REFERENCES dbo.Candidate(CandidateId)
            ON DELETE CASCADE
    );
    PRINT 'Table [dbo].[Address] created successfully.';
END
GO

IF OBJECT_ID('dbo.Education', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Education](

	[Id] INT IDENTITY(1,1) CONSTRAINT PK_Education PRIMARY KEY,
	
    [CandidateId] INT NOT NULL,
	
    [Specification] NVARCHAR(100) NOT NULL,
	
    [InstitutionName] NVARCHAR(255) NOT NULL,
	
    [Address] NVARCHAR(255) NOT NULL,
	
    [StartDate] DATETIME2 NOT NULL,
	
    [EndDate] DATETIME2 NULL,
	
    [CGPA] DECIMAL(3,2) NULL

    CONSTRAINT FK_Education_Candidate
            FOREIGN KEY (CandidateId)
            REFERENCES dbo.Candidate(CandidateId)
            ON DELETE CASCADE
);
PRINT 'Table [dbo].[Education] created successfully.';
END
GO

IF OBJECT_ID('dbo.Experience', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Experience](
	[Id] INT Primary key IDENTITY(1,1),
	[CandidateId] INT NOT NULL,
	[CompanyName] NVARCHAR(255) NOT NULL,
	[Designation] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(255) NULL,
	[StartDate] DATETIME2 NOT NULL,
	[EndDate] DATETIME2 NULL,
	[CTC] DECIMAL(18,2) NULL
);
END
GO

IF OBJECT_ID('dbo.Skill', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Skill](
	[Id] INT Primary key IDENTITY(1,1),
	[CandidateId] INT NOT NULL,
	[SkillName] NVARCHAR(100) NOT NULL,
	[Proficiency] INT NULL,
	[YearsOfExperience] DECIMAL(2,0) NULL,
	[StartDate] DATETIME2 NOT NULL,
	[EndDate] DATETIME2 NULL
);	
END
GO

IF OBJECT_ID('dbo.Language', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Language](
	[Id] INT IDENTITY(1,1) CONSTRAINT [PK-Language] PRIMARY KEY,
	[CandidateId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Proficiency] INT NOT NULL
);
END
GO