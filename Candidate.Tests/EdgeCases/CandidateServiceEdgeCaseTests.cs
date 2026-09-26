using Xunit;
using Moq;
using Candidate.Application.Services;
using Candidate.Infrastructure.Repositories;
using Candidate.Domain.Model;
using Candidate.Tests.Fixtures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Candidate.Tests.EdgeCases
{
    public class CandidateServiceEdgeCaseTests
    {
        private readonly Mock<ICandidateRepository> _mockRepository;
        private readonly CandidateService _service;

        public CandidateServiceEdgeCaseTests()
        {
            _mockRepository = new Mock<ICandidateRepository>();
            _service = new CandidateService(_mockRepository.Object);
        }

        #region Null and Empty Input Tests

        [Fact]
        public async Task AddCandidate_WithNullCandidate_ThrowsArgumentNullException()
        {
            // Arrange
            Domain.Model.Candidate nullCandidate = null!;
            _mockRepository
                .Setup(x => x.InsertCandidate(null!))
                .ThrowsAsync(new ArgumentNullException(nameof(nullCandidate)));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.AddCandidateProfile(nullCandidate)
            );
        }

        [Fact]
        public async Task AddCandidate_WithEmptyFirstName_StillAllowed()
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate(firstName: "");
            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert - Service should still process it
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task GetCandidateProfile_WithEmptyGuid_ThrowsInvalidOperationException()
        {
            // Arrange
            var emptyGuid = Guid.Empty;
            _mockRepository
                .Setup(x => x.GetCandidateProfileById(emptyGuid))
                .ThrowsAsync(new InvalidOperationException("Candidate not found"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GetCandidateProfileById(emptyGuid)
            );
        }

        #endregion

        #region User ID Uniqueness Tests

        [Fact]
        public async Task AddCandidate_WithDuplicateUserId_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidate = CandidateTestDataFactory.CreateValidCandidate(userId: userId);

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
        public async Task MultipleOperations_WithDifferentUserIds_AllSucceed()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var userId3 = Guid.NewGuid();

            var candidate1 = CandidateTestDataFactory.CreateValidCandidate(userId: userId1, firstName: "John");
            var candidate2 = CandidateTestDataFactory.CreateValidCandidate(userId: userId2, firstName: "Jane");
            var candidate3 = CandidateTestDataFactory.CreateValidCandidate(userId: userId3, firstName: "Bob");

            _mockRepository
                .Setup(x => x.InsertCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync((Domain.Model.Candidate c) => c);

            _mockRepository
                .Setup(x => x.GetCandidateProfileById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid userId) => new Domain.Model.Candidate 
                { 
                    UserId = userId,
                    Address = new List<Address>(),
                    Languages = new List<Language>(),
                    Educations = new List<Education>(),
                    Skills = new List<Skill>(),
                    Experiences = new List<WorkExperience>()
                });

            // Act
            var result1 = await _service.AddCandidateProfile(candidate1);
            var result2 = await _service.AddCandidateProfile(candidate2);
            var result3 = await _service.AddCandidateProfile(candidate3);

            // Assert
            Assert.All(new[] { result1, result2, result3 }, r => Assert.Equal(200, r.StatusCode));
        }

        #endregion

        #region Large Data Set Tests

        [Fact]
        public async Task UpdateCandidate_WithLargeNumberOfRelatedEntities_Succeeds()
        {
            // Arrange
            var candidate = CandidateBuilder.Create()
                .WithFirstName("LargeDataTest")
                .WithAddresses(CandidateTestDataFactory.CreateAddresses(10))
                .WithLanguages(CandidateTestDataFactory.CreateLanguages(6))
                .WithEducations(CandidateTestDataFactory.CreateEducations(5))
                .WithSkills(CandidateTestDataFactory.CreateSkills(20))
                .WithExperiences(CandidateTestDataFactory.CreateWorkExperiences(10))
                .Build();

            _mockRepository
                .Setup(x => x.UpdateCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.UpdateCandidateProfile(candidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(10, candidate.Address.Count);
            Assert.Equal(6, candidate.Languages.Count);
            Assert.Equal(5, candidate.Educations.Count);
            Assert.Equal(8, candidate.Skills.Count);
            Assert.Equal(6, candidate.Experiences.Count);
        }

        [Fact]
        public async Task DeleteCandidate_WithComplexProfile_RemovesAllData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidateWithData = CandidateTestDataFactory.CreateCandidateWithCompleteProfile(userId);

            _mockRepository
                .Setup(x => x.DeleteCandidate(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCandidateProfile(userId);

            // Assert
            Assert.Equal(200, result.StatusCode);
            _mockRepository.Verify(x => x.DeleteCandidate(userId), Times.Once);
        }

        #endregion

        #region Data Type and Format Tests

        [Theory]
        [InlineData("john@example.com")]
        [InlineData("jane.doe@company.co.uk")]
        [InlineData("test+tag@example.com")]
        public async Task AddCandidate_WithVariousEmailFormats_Accepted(string email)
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate();
            candidate.Email = email;

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData("1234567890")]
        [InlineData("+1-234-567-8900")]
        [InlineData("(123) 456-7890")]
        public async Task AddCandidate_WithVariousPhoneFormats_Accepted(string phoneNumber)
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate();
            candidate.PhoneNumber = phoneNumber;

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData("1950-01-01")]
        [InlineData("2000-06-15")]
        [InlineData("2010-12-31")]
        public async Task AddCandidate_WithVariousDOBFormats_Accepted(string dobString)
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate();
            candidate.DOB = DateTime.Parse(dobString);

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(DateTime.Parse(dobString), candidate.DOB);
        }

        #endregion

        #region Concurrency and Order Tests

        [Fact]
        public async Task SequentialOperations_MaintainDataIntegrity()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var candidate = CandidateBuilder.Create()
                .WithFirstName("Initial")
                .Build();
            candidate.UserId = userId;

            var updatedCandidate = CandidateBuilder.Create()
                .WithFirstName("Updated")
                .Build();
            updatedCandidate.UserId = userId;
            updatedCandidate.CandidateId = 1;

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            _mockRepository
                .Setup(x => x.GetCandidateProfileById(userId))
                .ReturnsAsync(() => updatedCandidate);

            _mockRepository
                .Setup(x => x.UpdateCandidate(updatedCandidate))
                .ReturnsAsync(updatedCandidate);

            // Act
            var addResult = await _service.AddCandidateProfile(candidate);
            var getResult = await _service.GetCandidateProfileById(userId);
            var updateResult = await _service.UpdateCandidateProfile(updatedCandidate);

            // Assert
            Assert.Equal(200, addResult.StatusCode);
            Assert.NotNull(getResult);
            Assert.Equal(200, updateResult.StatusCode);
        }

        #endregion

        #region Status Code and Message Consistency Tests

        [Fact]
        public async Task All_SuccessfulOperations_ReturnStatusCode200()
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate();
            candidate.CandidateId = 1;

            _mockRepository
                .Setup(x => x.InsertCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync(candidate);
            _mockRepository
                .Setup(x => x.UpdateCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync(candidate);
            _mockRepository
                .Setup(x => x.DeleteCandidate(It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var addResult = await _service.AddCandidateProfile(candidate);
            var updateResult = await _service.UpdateCandidateProfile(candidate);
            var deleteResult = await _service.DeleteCandidateProfile(candidate.UserId);

            // Assert
            Assert.Equal(200, addResult.StatusCode);
            Assert.Equal(200, updateResult.StatusCode);
            Assert.Equal(200, deleteResult.StatusCode);
        }

        [Fact]
        public async Task SuccessfulOperations_ContainNonEmptyMessages()
        {
            // Arrange
            var candidate = CandidateTestDataFactory.CreateValidCandidate();
            _mockRepository
                .Setup(x => x.InsertCandidate(It.IsAny<Domain.Model.Candidate>()))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.NotNull(result.StatusMessage);
            Assert.NotEmpty(result.StatusMessage);
            Assert.Contains("successfully", result.StatusMessage);
        }

        #endregion

        #region Builder Pattern Tests

        [Fact]
        public async Task CandidateBuilder_CreatesValidCandidateCorrectly()
        {
            // Arrange
            var candidate = CandidateBuilder.Create()
                .WithFirstName("BuilderTest")
                .WithLastName("Candidate")
                .WithEmail("builder@test.com")
                .WithCompleteProfile()
                .Build();

            _mockRepository
                .Setup(x => x.InsertCandidate(candidate))
                .ReturnsAsync(candidate);

            // Act
            var result = await _service.AddCandidateProfile(candidate);

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("BuilderTest", candidate.FirstName);
            Assert.Equal("Candidate", candidate.LastName);
            Assert.True(candidate.Address.Count > 0);
            Assert.True(candidate.Languages.Count > 0);
        }

        #endregion
    }
}
