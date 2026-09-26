IF OBJECT_ID('dbo.Language', 'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Language](
	[CandidateId] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Proficiency] INT NOT NULL
);
END