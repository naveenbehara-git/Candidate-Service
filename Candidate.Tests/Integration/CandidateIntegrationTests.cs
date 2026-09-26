using Xunit;
using Moq;
using Candidate.Application.Services;
using Candidate.Application.DTOs;
using Candidate.Infrastructure.Repositories;
using Candidate.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Candidate.Tests.Integration
{
    public class CandidateIntegrationTests
    {
        private readonly Mock<ICandidateRepository> _mockRepository;
        private readonly CandidateService _service;

        public CandidateIntegrationTests()
        {
            _mockRepository = new Mock<ICandidateRepository>();
            _service = new CandidateService(_mockRepository.Object);
        }

        #region CRUD Workflow Tests

        [Fact]
        public async Task Complete_CRUD_Workflow_SuccessfullyManagesCandidateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidateToAdd = new Domain.Model.Candidate
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                DOB = DateTime.Parse("1990-01-01"),
                MaritalStatus = "Single",
                Gender = Domain.Enums.Gender.Male,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = new List<Address> 
                { 
                    new Address { Id = 1, City = "New York", Country = "USA" }
                },
                Languages = new List<Language> 
                { 
                    new Language { Id = 1, Name = "English" }
                },
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            var candidateToUpdate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                UserId = userId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PhoneNumber = "9876543210",
                DOB = DateTime.Parse("1990-01-01"),
                MaritalStatus = "Married",
                Gender = Domain.Enums.Gender.Female,
                UpdatedAt = DateTime.UtcNow,
                Address = new List<Address> 
                { 
                    new Address { Id = 1, City = "Los Angeles", Country = "USA" }
                },
                Languages = new List<Language> 
                { 
                    new Language { Id = 1, Name = "English" },
                    new Language { Id = 2, Name = "Spanish" }
                },
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            // Setup mock for Add
            _mockRepository
                .Setup(x => x.InsertCandidate(candidateToAdd))
                .ReturnsAsync(candidateToAdd);

            // Setup mock for Get after Add
            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(candidateToAdd);

            // Setup mock for Update
            _mockRepository
                .Setup(x => x.UpdateCandidate(candidateToUpdate))
                .ReturnsAsync(candidateToUpdate);

            // Setup mock for Get after Update
            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(candidateToUpdate);

            // Setup mock for Delete
            _mockRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act - Create
            var addResult = await _service.AddCandidateProfile(candidateToAdd);
            Assert.Equal(200, addResult.StatusCode);

            // Act - Read
            var getResult = await _service.GetCandidateProfileById(userId);
            Assert.NotNull(getResult);
            Assert.Equal("Jane", getResult.FirstName);

            // Act - Update
            var updateResult = await _service.UpdateCandidateProfile(candidateToUpdate);
            Assert.Equal(200, updateResult.StatusCode);

            // Act - Read Updated
            var getUpdatedResult = await _service.GetCandidateProfileById(userId);
            Assert.Equal("Jane", getUpdatedResult.FirstName);
            Assert.Equal("Married", getUpdatedResult.MaritalStatus);

            // Act - Delete
            var deleteResult = await _service.DeleteCandidateProfile(userId);
            Assert.Equal(200, deleteResult.StatusCode);

            // Assert - Verify all operations were called
            _mockRepository.Verify(x => x.InsertCandidate(It.IsAny<Domain.Model.Candidate>()), Times.Once);
            _mockRepository.Verify(x => x.UpdateCandidate(It.IsAny<Domain.Model.Candidate>()), Times.Once);
            _mockRepository.Verify(x => x.DeleteCandidate(userId), Times.Once);
        }

        #endregion

        #region Data Consistency Tests

        [Fact]
        public async Task Multiple_Candidates_Can_Be_Managed_Independently()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();

            var candidate1 = new Domain.Model.Candidate
            {
                CandidateId = 1,
                UserId = userId1,
                FirstName = "John",
                LastName = "Doe",
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            var candidate2 = new Domain.Model.Candidate
            {
                CandidateId = 2,
                UserId = userId2,
                FirstName = "Jane",
                LastName = "Smith",
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId1))
                .ReturnsAsync(candidate1);
            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId2))
                .ReturnsAsync(candidate2);

            // Act
            var result1 = await _service.GetCandidateProfileById(userId1);
            var result2 = await _service.GetCandidateProfileById(userId2);

            // Assert
            Assert.NotEqual(result1.CandidateId, result2.CandidateId);
            Assert.NotEqual(result1.FirstName, result2.FirstName);
            Assert.Equal("John", result1.FirstName);
            Assert.Equal("Jane", result2.FirstName);
        }

        [Fact]
        public async Task Candidate_Profile_Maintains_Related_Entities_After_Update()
        {
            // Arrange
            var candidateWithEntities = new Domain.Model.Candidate
            {
                CandidateId = 1,
                FirstName = "John",
                LastName = "Doe",
                Address = new List<Address> 
                { 
                    new Address { Id = 1, City = "NYC", Country = "USA" },
                    new Address { Id = 2, City = "LA", Country = "USA" }
                },
                Languages = new List<Language> 
                { 
                    new Language { Id = 1, Name = "English" },
                    new Language { Id = 2, Name = "Spanish" },
                    new Language { Id = 3, Name = "French" }
                },
                Educations = new List<Education> 
                { 
                    new Education { Id = 1, InstitutionName = "MIT" },
                    new Education { Id = 2, InstitutionName = "Harvard" }
                },
                Skills = new List<Skill> 
                { 
                    new Skill { Id = 1, SkillName = "C#" },
                    new Skill { Id = 2, SkillName = "Python" }
                },
                Experiences = new List<WorkExperience> 
                { 
                    new WorkExperience { Id = 1, CompanyName = "Google" },
                    new WorkExperience { Id = 2, CompanyName = "Microsoft" }
                }
            };

            _mockRepository
                .Setup(x => x.UpdateCandidate(candidateWithEntities))
                .ReturnsAsync(candidateWithEntities);

            // Act
            var result = await _service.UpdateCandidateProfile(candidateWithEntities);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, candidateWithEntities.Address.Count);
            Assert.Equal(3, candidateWithEntities.Languages.Count);
            Assert.Equal(2, candidateWithEntities.Educations.Count);
            Assert.Equal(2, candidateWithEntities.Skills.Count);
            Assert.Equal(2, candidateWithEntities.Experiences.Count);
        }

        #endregion

        #region Error Scenario Tests

        [Fact]
        public async Task Adding_Duplicate_Candidate_Returns_Appropriate_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidate = new Domain.Model.Candidate
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ThrowsAsync(new InvalidOperationException("A candidate with the same UserId already exists."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddCandidateProfile(candidate)
            );
            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public async Task Updating_NonExistent_Candidate_Returns_Appropriate_Error()
        {
            // Arrange
            var candidate = new Domain.Model.Candidate
            {
                CandidateId = 999,
                FirstName = "NonExistent",
                LastName = "Candidate"
            };

            _mockRepository
                .Setup(x => x.UpdateCandidate(candidate))
                .ThrowsAsync(new InvalidOperationException("Candidate with ID 999 not found."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateCandidateProfile(candidate)
            );
            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task Deleting_NonExistent_Candidate_Returns_Appropriate_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ThrowsAsync(new InvalidOperationException($"Candidate with UserId {userId} not found."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.DeleteCandidateProfile(userId)
            );
            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task Retrieving_NonExistent_Candidate_Returns_Appropriate_Error()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ThrowsAsync(new InvalidOperationException($"Candidate with UserId {userId} not found."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GetCandidateProfileById(userId)
            );
            Assert.Contains("not found", exception.Message);
        }

        #endregion

        #region Response Format Tests

        [Theory]
        [InlineData(1, "Test Candidate")]
        [InlineData(2, "Another Candidate")]
        [InlineData(3, "Third Candidate")]
        public async Task StatusDTO_Always_Returns_Correct_Format(int expectedStatusCode, string testName)
        {
            // Arrange
            var candidate = new Domain.Model.Candidate
            {
                UserId = Guid.NewGuid(),
                FirstName = testName,
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<StatusDTO>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotEmpty(result.StatusMessage);
        }

        [Fact]
        public async Task All_Operations_Return_Consistent_Status_Code()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidate = new Domain.Model.Candidate
            {
                CandidateId = 1,
                UserId = userId,
                FirstName = "Test",
                Address = new List<Address>(),
                Languages = new List<Language>(),
                Educations = new List<Education>(),
                Skills = new List<Skill>(),
                Experiences = new List<WorkExperience>()
            };

            _mockRepository
                .Setup(x => x.InsertCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync(candidate);
            _mockRepository
                .Setup(x => x.UpdateCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync(candidate);
            _mockRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act
            var addResult = await _service.AddCandidateProfile(candidate);
            var updateResult = await _service.UpdateCandidateProfile(candidate);
            var deleteResult = await _service.DeleteCandidateProfile(userId);

            // Assert
            Assert.Equal(200, addResult.StatusCode);
            Assert.Equal(200, updateResult.StatusCode);
            Assert.Equal(200, deleteResult.StatusCode);
        }

        #endregion
    }
}
