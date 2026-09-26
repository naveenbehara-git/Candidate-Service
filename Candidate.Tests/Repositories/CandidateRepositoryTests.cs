using Candidate.Domain.Enums;
using Candidate.Domain.Model;
using Candidate.Infrastructure.Data;
using Candidate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

namespace Candidate.Tests.Repositories
{
    public class CandidateRepositoryTests : IDisposable
    {
        private readonly DatabaseService _databaseService;
        private readonly CandidateRepository _repository;

        public CandidateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseService>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseService = new DatabaseService(options);
            _repository = new CandidateRepository(_databaseService);
        }

        public void Dispose()
        {
            _databaseService.Database.EnsureDeleted();
            _databaseService.Dispose();
        }

        #region GetCandidateProfileById

        [Fact]
        public async Task GetCandidateProfileById_WithValidUserId_ReturnsCandidate()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var candidate = CreateCandidate(userId);

            _databaseService.Candidate.Add(candidate);
            await _databaseService.SaveChangesAsync();

            // Act
            var result = await _repository.GetCandidateProfileById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john@example.com", result.Email);
        }

        [Fact]
        public async Task GetCandidateProfileById_WithInvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _repository.GetCandidateProfileById(userId));

            // Assert
            Assert.Equal(
                "An error occurred while retrieving the candidate profile.",
                exception.Message);

            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public async Task GetCandidateProfileById_WithRelatedEntities_ReturnsAllRelatedEntities()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var candidate = CreateCandidate(userId);

            candidate.Address.Add(new Address
            {
                City = "New York",
                Country = "USA"
            });

            candidate.Languages.Add(new Language
            {
                Name = "English"
            });

            candidate.Educations.Add(new Education
            {
                InstitutionName = "MIT"
            });

            candidate.Skills.Add(new Skill
            {
                SkillName = "C#"
            });

            candidate.Experiences.Add(new WorkExperience
            {
                CompanyName = "Google"
            });

            _databaseService.Candidate.Add(candidate);
            await _databaseService.SaveChangesAsync();

            // Act
            var result = await _repository.GetCandidateProfileById(userId);

            // Assert
            Assert.NotNull(result);

            Assert.Single(result.Address);
            Assert.Equal("New York", result.Address.First().City);

            Assert.Single(result.Languages);
            Assert.Equal("English", result.Languages.First().Name);

            Assert.Single(result.Educations);
            Assert.Equal("MIT", result.Educations.First().InstitutionName);

            Assert.Single(result.Skills);
            Assert.Equal("C#", result.Skills.First().SkillName);

            Assert.Single(result.Experiences);
            Assert.Equal("Google", result.Experiences.First().CompanyName);
        }

        #endregion


        #region InsertCandidate

        [Fact]
        public async Task InsertCandidate_WithValidCandidate_InsertsSuccessfully()
        {
            // Arrange
            var candidate = CreateCandidate(
                Guid.NewGuid(),
                "Alice",
                "Smith",
                "alice@example.com");

            // Act
            var result = await _repository.InsertCandidate(candidate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.Equal("alice@example.com", result.Email);

            var savedCandidate = await _databaseService.Candidate
                .FirstOrDefaultAsync(x => x.UserId == candidate.UserId);

            Assert.NotNull(savedCandidate);
        }

        [Fact]
        public async Task InsertCandidate_WithDuplicateUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingCandidate = CreateCandidate(
                userId,
                "Existing",
                "User",
                "existing@example.com");

            _databaseService.Candidate.Add(existingCandidate);
            await _databaseService.SaveChangesAsync();

            var duplicateCandidate = CreateCandidate(
                userId,
                "New",
                "User",
                "new@example.com");

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _repository.InsertCandidate(duplicateCandidate));

            // Assert
            Assert.Equal(
                "An error occurred while inserting the candidate.",
                exception.Message);

            Assert.IsType<InvalidOperationException>(exception.InnerException);

            Assert.Contains(
                "same UserId",
                exception.InnerException!.Message);
        }

        [Fact]
        public async Task InsertCandidate_WithRelatedEntities_InsertsAllEntities()
        {
            // Arrange
            var candidate = CreateCandidate(
                Guid.NewGuid(),
                "Bob",
                "Johnson",
                "bob@example.com");

            candidate.Address.Add(new Address
            {
                City = "San Francisco",
                Country = "USA"
            });

            candidate.Address.Add(new Address
            {
                City = "Austin",
                Country = "USA"
            });

            candidate.Languages.Add(new Language
            {
                Name = "English"
            });

            candidate.Educations.Add(new Education
            {
                InstitutionName = "Stanford"
            });

            candidate.Skills.Add(new Skill
            {
                SkillName = "Python"
            });

            candidate.Experiences.Add(new WorkExperience
            {
                CompanyName = "Apple"
            });

            // Act
            var result = await _repository.InsertCandidate(candidate);

            // Assert
            Assert.NotNull(result);

            var savedCandidate = await _databaseService.Candidate
                .Include(x => x.Address)
                .Include(x => x.Languages)
                .Include(x => x.Educations)
                .Include(x => x.Skills)
                .Include(x => x.Experiences)
                .FirstAsync(x => x.UserId == candidate.UserId);

            Assert.Equal(2, savedCandidate.Address.Count);
            Assert.Single(savedCandidate.Languages);
            Assert.Single(savedCandidate.Educations);
            Assert.Single(savedCandidate.Skills);
            Assert.Single(savedCandidate.Experiences);
        }

        #endregion


        #region UpdateCandidate

        [Fact]
        public async Task UpdateCandidate_WithValidCandidate_UpdatesCandidate()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingCandidate = CreateCandidate(
                userId,
                "John",
                "Doe",
                "john@example.com");

            _databaseService.Candidate.Add(existingCandidate);
            await _databaseService.SaveChangesAsync();

            var update = new Candidate.Domain.Model.Candidate
            {
                CandidateId = existingCandidate.CandidateId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com"
            };

            // Act
            var result = await _repository.UpdateCandidate(update);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Smith", result.LastName);
            Assert.Equal("jane@example.com", result.Email);

            var savedCandidate = await _databaseService.Candidate
                .AsNoTracking()
                .FirstAsync(x => x.CandidateId == existingCandidate.CandidateId);

            Assert.Equal("Jane", savedCandidate.FirstName);
            Assert.Equal("Smith", savedCandidate.LastName);
            Assert.Equal("jane@example.com", savedCandidate.Email);
        }

        [Fact]
        public async Task UpdateCandidate_WithNonExistingCandidate_ThrowsException()
        {
            // Arrange
            var update = new Candidate.Domain.Model.Candidate
            {
                CandidateId = 999999,
                FirstName = "Test"
            };

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _repository.UpdateCandidate(update));

            // Assert
            Assert.Equal(
                "An error occurred while updating the candidate.",
                exception.Message);

            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public async Task UpdateCandidate_WithPartialData_PreservesExistingValues()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingCandidate = CreateCandidate(
                userId,
                "John",
                "Doe",
                "john@example.com");

            existingCandidate.PhoneNumber = "1234567890";
            existingCandidate.MaritalStatus = "Married";

            _databaseService.Candidate.Add(existingCandidate);
            await _databaseService.SaveChangesAsync();

            var update = new Candidate.Domain.Model.Candidate
            {
                CandidateId = existingCandidate.CandidateId,
                FirstName = "Jane"
            };

            // Act
            var result = await _repository.UpdateCandidate(update);

            // Assert
            Assert.Equal("Jane", result.FirstName);

            // Repository uses ?? for these properties,
            // so existing values should remain unchanged.
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("1234567890", result.PhoneNumber);
            Assert.Equal("Married", result.MaritalStatus);
        }

        [Fact]
        public async Task UpdateCandidate_UpdatesUpdatedAt()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var oldDate = DateTime.UtcNow.AddDays(-1);

            var existingCandidate = CreateCandidate(userId);
            existingCandidate.UpdatedAt = oldDate;

            _databaseService.Candidate.Add(existingCandidate);
            await _databaseService.SaveChangesAsync();

            var update = new Candidate.Domain.Model.Candidate
            {
                CandidateId = existingCandidate.CandidateId,
                FirstName = "Updated"
            };

            // Act
            var result = await _repository.UpdateCandidate(update);

            // Assert
            Assert.True(result.UpdatedAt > oldDate);
        }

        [Fact]
        public async Task UpdateCandidate_WithRelatedEntities_ReplacesRelatedEntities()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var existingCandidate = CreateCandidate(userId);

            existingCandidate.Address.Add(new Address
            {
                City = "Old City",
                Country = "USA"
            });

            existingCandidate.Languages.Add(new Language
            {
                Name = "English"
            });

            _databaseService.Candidate.Add(existingCandidate);
            await _databaseService.SaveChangesAsync();

            var update = new Candidate.Domain.Model.Candidate
            {
                CandidateId = existingCandidate.CandidateId,
                FirstName = "Updated"
            };

            update.Address.Add(new Address
            {
                City = "New City",
                Country = "India"
            });

            update.Languages.Add(new Language
            {
                Name = "Hindi"
            });

            // Act
            await _repository.UpdateCandidate(update);

            // Assert
            var savedCandidate = await _databaseService.Candidate
                .Include(x => x.Address)
                .Include(x => x.Languages)
                .FirstAsync(x => x.CandidateId == existingCandidate.CandidateId);

            Assert.Single(savedCandidate.Address);
            Assert.Equal("New City", savedCandidate.Address.First().City);

            Assert.Single(savedCandidate.Languages);
            Assert.Equal("Hindi", savedCandidate.Languages.First().Name);
        }

        #endregion


        #region DeleteCandidate

        [Fact]
        public async Task DeleteCandidate_WithValidUserId_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var candidate = CreateCandidate(userId);

            _databaseService.Candidate.Add(candidate);
            await _databaseService.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteCandidate(userId);

            // Assert
            Assert.True(result);

            var deletedCandidate = await _databaseService.Candidate
                .FirstOrDefaultAsync(x => x.UserId == userId);

            Assert.Null(deletedCandidate);
        }

        [Fact]
        public async Task DeleteCandidate_WithInvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _repository.DeleteCandidate(userId));

            // Assert
            Assert.Equal(
                "An error occurred while deleting the candidate.",
                exception.Message);

            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        [Fact]
        public async Task DeleteCandidate_WithRelatedEntities_DeletesRelatedEntities()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var candidate = CreateCandidate(userId);

            candidate.Address.Add(new Address
            {
                City = "Boston",
                Country = "USA"
            });

            candidate.Languages.Add(new Language
            {
                Name = "Spanish"
            });

            candidate.Educations.Add(new Education
            {
                InstitutionName = "Harvard"
            });

            candidate.Skills.Add(new Skill
            {
                SkillName = "Java"
            });

            candidate.Experiences.Add(new WorkExperience
            {
                CompanyName = "Microsoft"
            });

            _databaseService.Candidate.Add(candidate);
            await _databaseService.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteCandidate(userId);

            // Assert
            Assert.True(result);

            Assert.Empty(await _databaseService.Address.ToListAsync());
            Assert.Empty(await _databaseService.Language.ToListAsync());
            Assert.Empty(await _databaseService.Education.ToListAsync());
            Assert.Empty(await _databaseService.Skill.ToListAsync());
            Assert.Empty(await _databaseService.Experience.ToListAsync());
            Assert.Empty(await _databaseService.Candidate.ToListAsync());
        }

        #endregion


        #region Test Data Helpers

        private static Candidate.Domain.Model.Candidate CreateCandidate(
            Guid userId,
            string firstName = "John",
            string lastName = "Doe",
            string email = "john@example.com")
        {
            return new Candidate.Domain.Model.Candidate
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = "1234567890",
                DOB = new DateTime(1995, 5, 15),
                MaritalStatus = "Single",
                Gender = Gender.Male,

                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };
        }

        #endregion
    }
}

