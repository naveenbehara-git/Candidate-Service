using Candidate.Domain.Model;
using System;
using System.Collections.Generic;

namespace Candidate.Tests.Fixtures
{
    /// <summary>
    /// Factory class to generate test data for Candidate entities.
    /// Provides reusable test fixtures for unit and integration tests.
    /// </summary>
    public class CandidateTestDataFactory
    {
        /// <summary>
        /// Creates a valid candidate with minimal properties.
        /// </summary>
        public static Domain.Model.Candidate CreateValidCandidate(
            Guid? userId = null,
            int candidateId = 1,
            string firstName = "John",
            string lastName = "Doe")
        {
            return new Domain.Model.Candidate
            {
                CandidateId = candidateId,
                UserId = userId ?? Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}@example.com",
                PhoneNumber = "1234567890",
                DOB = DateTime.Parse("1990-01-01"),
                Gender = Domain.Enums.Gender.Male,
                MaritalStatus = "Single",
                Certifications = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };
        }

        /// <summary>
        /// Creates a candidate with complete profile including all related entities.
        /// </summary>
        public static Domain.Model.Candidate CreateCandidateWithCompleteProfile(Guid? userId = null)
        {
            var candidate = CreateValidCandidate(userId);
            candidate.Address = CreateAddresses();
            candidate.Languages = CreateLanguages();
            candidate.Educations = CreateEducations();
            candidate.Skills = CreateSkills();
            candidate.Experiences = CreateWorkExperiences();
            return candidate;
        }

        /// <summary>
        /// Creates a list of addresses for testing.
        /// </summary>
        public static List<Address> CreateAddresses(int count = 2)
        {
            var addresses = new List<Address>();
            var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" };
            var countries = new[] { "USA", "Canada", "UK", "Australia", "Germany" };

            for (int i = 0; i < count; i++)
            {
                addresses.Add(new Address
                {
                    Id = i + 1,
                    Address1 = $"{100 + i} Main Street",
                    Address2 = $"{100 + i} Secondary Street",
                    City = cities[i % cities.Length],
                    State = "CA",
                    PostalCode = 90000 + i,
                    Country = countries[i % countries.Length]
                });
            }

            return addresses;
        }

        /// <summary>
        /// Creates a list of languages for testing.
        /// </summary>
        public static List<Language> CreateLanguages(int count = 3)
        {
            var languages = new List<Language>();
            var languageNames = new[] { "English", "Spanish", "French", "German", "Mandarin", "Japanese" };

            for (int i = 0; i < Math.Min(count, languageNames.Length); i++)
            {
                languages.Add(new Language
                {
                    Id = i + 1,
                    Name = languageNames[i],
                    Proficiency = i+ 1,
                });
            }

            return languages;
        }

        /// <summary>
        /// Creates a list of educations for testing.
        /// </summary>
        public static List<Education> CreateEducations(int count = 2)
        {
            var educations = new List<Education>();
            var institutions = new[] { "MIT", "Harvard", "Stanford", "Yale", "Princeton" };
            var degrees = new[] { "Bachelor", "Master", "PhD" };

            for (int i = 0; i < Math.Min(count, institutions.Length); i++)
            {
                educations.Add(new Education
                {
                    Id = i + 1,
                    InstitutionName = institutions[i],
                    Specification = "Computer Science",
                    StartDate = DateTime.Parse($"2010-09-01").AddYears(i),
                    EndDate = DateTime.Parse($"2014-05-31").AddYears(i)
                });
            }

            return educations;
        }

        /// <summary>
        /// Creates a list of skills for testing.
        /// </summary>
        public static List<Skill> CreateSkills(int count = 5)
        {
            var skills = new List<Skill>();
            var skillNames = new[] { "C#", "Python", "JavaScript", "SQL", "ASP.NET", "React", "Docker", "Kubernetes" };

            for (int i = 0; i < Math.Min(count, skillNames.Length); i++)
            {
                skills.Add(new Skill
                {
                    Id = i + 1,
                    SkillName = skillNames[i],
                    Proficiency = 2
                });
            }

            return skills;
        }

        /// <summary>
        /// Creates a list of work experiences for testing.
        /// </summary>
        public static List<WorkExperience> CreateWorkExperiences(int count = 2)
        {
            var experiences = new List<WorkExperience>();
            var companies = new[] { "Google", "Microsoft", "Amazon", "Apple", "Meta", "Tesla" };
            var positions = new[] { "Software Engineer", "Senior Developer", "Tech Lead", "Principal Engineer" };

            for (int i = 0; i < Math.Min(count, companies.Length); i++)
            {
                experiences.Add(new WorkExperience
                {
                    Id = i + 1,
                    CompanyName = companies[i],
                    Designation = positions[i % positions.Length],
                    StartDate = DateTime.Parse($"2015-01-01").AddYears(i),
                    EndDate = DateTime.Parse($"2018-12-31").AddYears(i),
                    Description = $"Worked as {positions[i % positions.Length]} at {companies[i]}"
                });
            }

            return experiences;
        }

        /// <summary>
        /// Creates multiple test candidates with unique data.
        /// </summary>
        public static List<Domain.Model.Candidate> CreateMultipleCandidates(int count = 5)
        {
            var candidates = new List<Domain.Model.Candidate>();
            var firstNames = new[] { "John", "Jane", "Robert", "Mary", "Michael", "Sarah", "David", "Emma" };
            var lastNames = new[] { "Doe", "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller" };

            for (int i = 0; i < count; i++)
            {
                var candidate = CreateValidCandidate(
                    candidateId: i + 1,
                    firstName: firstNames[i % firstNames.Length],
                    lastName: lastNames[i % lastNames.Length]
                );
                candidates.Add(candidate);
            }

            return candidates;
        }

        /// <summary>
        /// Creates a candidate with specific properties for testing edge cases.
        /// </summary>
        public static Domain.Model.Candidate CreateCandidateForUpdate()
        {
            return new Domain.Model.Candidate
            {
                CandidateId = 1,
                FirstName = "UpdatedFirstName",
                LastName = "UpdatedLastName",
                Email = "updated@example.com",
                PhoneNumber = "9876543210",
                Gender = Domain.Enums.Gender.Female,
                DOB = DateTime.Parse("1985-06-15"),
                MaritalStatus = "Married",
                Certifications = "AWS Certified Solution Architect",
                UpdatedAt = DateTime.UtcNow,
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };
        }
    }

    /// <summary>
    /// Test data builder for fluent test setup.
    /// Allows building test candidates with method chaining.
    /// </summary>
    public class CandidateBuilder
    {
        private Domain.Model.Candidate _candidate;

        public CandidateBuilder()
        {
            _candidate = CandidateTestDataFactory.CreateValidCandidate();
        }

        public CandidateBuilder WithFirstName(string firstName)
        {
            _candidate.FirstName = firstName;
            return this;
        }

        public CandidateBuilder WithLastName(string lastName)
        {
            _candidate.LastName = lastName;
            return this;
        }

        public CandidateBuilder WithEmail(string email)
        {
            _candidate.Email = email;
            return this;
        }

        public CandidateBuilder WithPhoneNumber(string phoneNumber)
        {
            _candidate.PhoneNumber = phoneNumber;
            return this;
        }

        public CandidateBuilder WithGender(Domain.Enums.Gender gender)
        {
            _candidate.Gender = gender;
            return this;
        }

        public CandidateBuilder WithDOB(DateTime dob)
        {
            _candidate.DOB = dob;
            return this;
        }

        public CandidateBuilder WithMaritalStatus(string maritalStatus)
        {
            _candidate.MaritalStatus = maritalStatus;
            return this;
        }

        public CandidateBuilder WithAddresses(List<Address> addresses)
        {
            _candidate.Address = addresses;
            return this;
        }

        public CandidateBuilder WithLanguages(List<Language> languages)
        {
            _candidate.Languages = languages;
            return this;
        }

        public CandidateBuilder WithEducations(List<Education> educations)
        {
            _candidate.Educations = educations;
            return this;
        }

        public CandidateBuilder WithSkills(List<Skill> skills)
        {
            _candidate.Skills = skills;
            return this;
        }

        public CandidateBuilder WithExperiences(List<WorkExperience> experiences)
        {
            _candidate.Experiences = experiences;
            return this;
        }

        public CandidateBuilder WithCompleteProfile()
        {
            _candidate.Address = CandidateTestDataFactory.CreateAddresses();
            _candidate.Languages = CandidateTestDataFactory.CreateLanguages();
            _candidate.Educations = CandidateTestDataFactory.CreateEducations();
            _candidate.Skills = CandidateTestDataFactory.CreateSkills();
            _candidate.Experiences = CandidateTestDataFactory.CreateWorkExperiences();
            return this;
        }

        public Domain.Model.Candidate Build()
        {
            return _candidate;
        }

        public static CandidateBuilder Create()
        {
            return new CandidateBuilder();
        }
    }
}
