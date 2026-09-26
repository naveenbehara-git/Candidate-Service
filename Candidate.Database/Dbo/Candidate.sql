IF OBJECT_ID('dbo.Candidate', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Candidate]
    (
        CandidateId INT NOT NULL CONSTRAINT PK_Candidate PRIMARY KEY IDENTITY(1,1),

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