IF OBJECT_ID('dbo.Skill', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Skill](
	[Id] INT Primary key IDENTITY(1,1),
	[CandidateId] INT NOT NULL,
	[SkillName] NVARCHAR(100) NOT NULL,
	[Proficiency] NVARCHAR(50) NULL,
	[YearsOfExperience] INT NULL,
	[StartDate] DATETIME2 NOT NULL,
	[EndDate] DATETIME2 NULL
);	
END

