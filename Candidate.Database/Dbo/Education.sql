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

    CONSTRAINT FK_Address_Candidate
            FOREIGN KEY (CandidateId)
            REFERENCES dbo.Candidate(CandidateId)
            ON DELETE CASCADE
);
PRINT 'Table [dbo].[Address] created successfully.';
END
GO
