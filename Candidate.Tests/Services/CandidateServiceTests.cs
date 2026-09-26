using Xunit;
using Moq;
using Candidate.Application.Services;
using Candidate.Application.DTOs;
using Candidate.Infrastructure.Repositories;
using Candidate.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Candidate.Tests.Services
{
    public class CandidateServiceTests
    {
        private readonly Mock<ICandidateRepository> _mockCandidateRepository;
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _mockCandidateRepository = new Mock<ICandidateRepository>();
            _candidateService = new CandidateService(_mockCandidateRepository.Object);
        }

        #region GetCandidateProfileById Tests

        [Fact]
        public async Task GetCandidateProfileById_WithValidUserId_ReturnsCandidateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedCandidate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                DOB = DateTime.Parse("1990-01-01"),
                MaritalStatus = "Single",
                Gender = Domain.Enums.Gender.Male,
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockCandidateRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(expectedCandidate);

            // Act
            var result = await _candidateService.GetCandidateProfileById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            _mockCandidateRepository.Verify(x => x.GetCandidateProfileById(userId), Times.Once);
        }

        [Fact]
        public async Task GetCandidateProfileById_WithInvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCandidateRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ThrowsAsync(new InvalidOperationException("Candidate not found"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _candidateService.GetCandidateProfileById(userId)
            );
        }

        [Fact]
        public async Task GetCandidateProfileById_ReturnsCompleteCandidateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedCandidate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                UserId = userId,
                FirstName = "John",
                Address = new List<Address> 
                { 
                    new Address { City = "NYC", Country = "USA" }
                },
                Languages = new List<Language> 
                { 
                    new Language { Name = "English" },
                    new Language { Name = "Spanish" }
                },
                Educations = new List<Education> 
                { 
                    new Education { InstitutionName = "MIT" }
                },
                Skills = new List<Skill> 
                { 
                    new Skill { SkillName = "C#" },
                    new Skill { SkillName = "ASP.NET" }
                },
                Experiences = new List<WorkExperience> 
                { 
                    new WorkExperience { CompanyName = "Google" }
                }
            };

            _mockCandidateRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(expectedCandidate);

            // Act
            var result = await _candidateService.GetCandidateProfileById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Address);
            Assert.Equal(2, result.Languages.Count);
            Assert.Single(result.Educations);
            Assert.Equal(2, result.Skills.Count);
            Assert.Single(result.Experiences);
        }

        #endregion

        #region AddCandidateProfile Tests

        [Fact]
        public async Task AddCandidateProfile_WithValidCandidate_ReturnsSuccessStatusDTO()
        {
            // Arrange
            var newCandidate = new Domain.Model.Candidate
            {
                UserId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockCandidateRepository
                .Setup(x => x.InsertCandidate(newCandidate))
                .ReturnsAsync(newCandidate);

            // Act
            var result = await _candidateService.AddCandidateProfile(newCandidate);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StatusDTO>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Candidate profile added successfully.", result.StatusMessage);
            _mockCandidateRepository.Verify(x => x.InsertCandidate(newCandidate), Times.Once);
        }

        [Fact]
        public async Task AddCandidateProfile_WithDuplicateUserId_ReturnsErrorStatus()
        {
            // Arrange
            var newCandidate = new Domain.Model.Candidate
            {
                UserId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe"
            };

            _mockCandidateRepository
                .Setup(x => x.InsertCandidate(newCandidate))
                .ThrowsAsync(new InvalidOperationException("A candidate with the same UserId already exists."));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _candidateService.AddCandidateProfile(newCandidate)
            );
        }

        [Fact]
        public async Task AddCandidateProfile_WithRelatedEntities_SuccessfullyAddsAll()
        {
            // Arrange
            var newCandidate = new Domain.Model.Candidate
            {
                UserId = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Address = new List<Address> 
                { 
                    new Address { City = "LA", Country = "USA" }
                },
                Languages = new List<Language> 
                { 
                    new Language { Name = "French" }
                },
                Educations = new List<Education> 
                { 
                    new Education { InstitutionName = "Harvard" }
                },
                Skills = new List<Skill> 
                { 
                    new Skill { SkillName = "Python" }
                },
                Experiences = new List<WorkExperience> 
                { 
                    new WorkExperience { CompanyName = "Microsoft" }
                }
            };

            _mockCandidateRepository
                .Setup(x => x.InsertCandidate(newCandidate))
                .ReturnsAsync(newCandidate);

            // Act
            var result = await _candidateService.AddCandidateProfile(newCandidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.StatusMessage);
            _mockCandidateRepository.Verify(x => x.InsertCandidate(newCandidate), Times.Once);
        }

        #endregion

        #region UpdateCandidateProfile Tests

        [Fact]
        public async Task UpdateCandidateProfile_WithValidCandidate_ReturnsSuccessStatusDTO()
        {
            // Arrange
            var candidateToUpdate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                FirstName = "Jane",
                LastName = "Updated",
                Email = "jane.updated@example.com",
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockCandidateRepository
                .Setup(x => x.UpdateCandidate(candidateToUpdate))
                .ReturnsAsync(candidateToUpdate);

            // Act
            var result = await _candidateService.UpdateCandidateProfile(candidateToUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StatusDTO>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Candidate profile updated successfully.", result.StatusMessage);
            _mockCandidateRepository.Verify(x => x.UpdateCandidate(candidateToUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateCandidateProfile_WithNonExistentCandidate_ThrowsException()
        {
            // Arrange
            var candidateToUpdate = new Domain.Model.Candidate
            {
                CandidateId = 999,
                FirstName = "NonExistent"
            };

            _mockCandidateRepository
                .Setup(x => x.UpdateCandidate(candidateToUpdate))
                .ThrowsAsync(new InvalidOperationException("Candidate with ID 999 not found."));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _candidateService.UpdateCandidateProfile(candidateToUpdate)
            );
        }

        [Fact]
        public async Task UpdateCandidateProfile_UpdatesAllProperties()
        {
            // Arrange
            var candidateToUpdate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                Email = "updated@example.com",
                PhoneNumber = "9876543210",
                MaritalStatus = "Married",
                Address = new List<Address> 
                { 
                    new Address { City = "Updated City", Country = "Updated Country" }
                },
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockCandidateRepository
                .Setup(x => x.UpdateCandidate(candidateToUpdate))
                .ReturnsAsync(candidateToUpdate);

            // Act
            var result = await _candidateService.UpdateCandidateProfile(candidateToUpdate);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.StatusMessage);
            _mockCandidateRepository.Verify(x => x.UpdateCandidate(candidateToUpdate), Times.Once);
        }

        #endregion

        #region DeleteCandidateProfile Tests

        [Fact]
        public async Task DeleteCandidateProfile_WithValidUserId_ReturnsSuccessStatusDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCandidateRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _candidateService.DeleteCandidateProfile(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StatusDTO>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Candidate profile deleted successfully.", result.StatusMessage);
            _mockCandidateRepository.Verify(x => x.DeleteCandidate(userId), Times.Once);
        }

        [Fact]
        public async Task DeleteCandidateProfile_WithInvalidUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCandidateRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ThrowsAsync(new InvalidOperationException($"Candidate with UserId {userId} not found."));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _candidateService.DeleteCandidateProfile(userId)
            );
        }

        [Fact]
        public async Task DeleteCandidateProfile_WithExistingCandidate_CallsRepositoryOnce()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockCandidateRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act
            await _candidateService.DeleteCandidateProfile(userId);

            // Assert
            _mockCandidateRepository.Verify(x => x.DeleteCandidate(userId), Times.Once);
        }

        #endregion

        #region Service Error Handling Tests

        [Fact]
        public async Task Service_Handle_ArgumentNullException()
        {
            // Arrange
            Guid nullUserId = Guid.Empty;
            _mockCandidateRepository
                .Setup(x => x.GetCandidateProfileById(Guid.Empty))
                .ThrowsAsync(new ArgumentNullException(nameof(nullUserId)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _candidateService.GetCandidateProfileById(nullUserId)
            );
        }

        [Fact]
        public async Task Service_AllMethods_ProperlyCallRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidate = new Domain.Model.Candidate 
            { 
                CandidateId = 1, 
                UserId = userId,
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockCandidateRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(candidate);
            _mockCandidateRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);
            _mockCandidateRepository
                .Setup(x => x.UpdateCandidate(candidate))
                .ReturnsAsync(candidate);
            _mockCandidateRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act
            await _candidateService.GetCandidateProfileById(userId);
            await _candidateService.AddCandidateProfile(candidate);
            await _candidateService.UpdateCandidateProfile(candidate);
            await _candidateService.DeleteCandidateProfile(userId);

            // Assert
            _mockCandidateRepository.Verify(x => x.GetCandidateProfileById(userId), Times.Once);
            _mockCandidateRepository.Verify(x => x.InsertCandidate(candidate), Times.Once);
            _mockCandidateRepository.Verify(x => x.UpdateCandidate(candidate), Times.Once);
            _mockCandidateRepository.Verify(x => x.DeleteCandidate(userId), Times.Once);
        }

        #endregion
    }
}
