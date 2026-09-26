IF OBJECT_ID('dbo.Address', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.[Address]
    (
        Id INT IDENTITY(1,1) CONSTRAINT PK_Address PRIMARY KEY,

        CandidateId INT NOT NULL,

        Address1 NVARCHAR(255) NOT NULL,

        Address2 NVARCHAR(255) NULL,

        City NVARCHAR(100) NULL,

        [State] NVARCHAR(100) NULL,

        Country NVARCHAR(100) NULL,

        PostalCode NVARCHAR(20) NULL,

        CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Address_CreatedAt DEFAULT SYSUTCDATETIME(),

        UpdatedAt DATETIME2 NOT NULL CONSTRAINT DF_Address_UpdatedAt DEFAULT SYSUTCDATETIME(),

        CONSTRAINT FK_Address_Candidate
            FOREIGN KEY (CandidateId)
            REFERENCES dbo.Candidate(CandidateId)
            ON DELETE CASCADE
    );
    PRINT 'Table [dbo].[Address] created successfully.';
END

