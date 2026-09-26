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
