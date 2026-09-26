# Candidate.Tests - Test Suite Documentation

## Overview

The **Candidate.Tests** project contains comprehensive unit and integration tests for the Candidate Service API. It uses **xUnit** as the testing framework and **Moq** for mocking dependencies.

## Project Structure

```
Candidate.Tests/
├── Repositories/
│   └── CandidateRepositoryTests.cs       # Repository unit tests
├── Services/
│   └── CandidateServiceTests.cs          # Service unit tests
├── Integration/
│   └── CandidateIntegrationTests.cs      # End-to-end integration tests
├── EdgeCases/
│   └── CandidateServiceEdgeCaseTests.cs  # Edge cases and boundary tests
├── Fixtures/
│   └── CandidateTestDataFactory.cs       # Test data generation utilities
└── Candidate.Tests.csproj               # Project configuration
```

## Dependencies

- **xUnit** (v2.6.2) - Testing framework
- **Moq** (v4.20.70) - Mocking library
- **Microsoft.NET.Test.Sdk** (v17.8.0) - Test SDK
- **coverlet.collector** (v6.0.0) - Code coverage

## Test Cases

### 1. Repository Tests (`CandidateRepositoryTests.cs`)

#### GetCandidateProfileByIdAsync
- ✅ Returns candidate profile with valid UserId
- ✅ Throws exception with invalid UserId
- ✅ Loads all related entities (Address, Languages, Educations, Skills, Experiences)

#### InsertCandidate
- ✅ Successfully inserts valid candidate
- ✅ Throws exception when duplicate UserId
- ✅ Inserts all related entities correctly

#### UpdateCandidateAsync
- ✅ Successfully updates valid candidate
- ✅ Throws exception for non-existent candidate
- ✅ Updates timestamp to current UTC time
- ✅ Preserves related entities during update

#### DeleteCandidateAsync
- ✅ Successfully deletes candidate with valid UserId
- ✅ Throws exception with invalid UserId
- ✅ Deletes all related entities (cascade delete)

### 2. Service Tests (`CandidateServiceTests.cs`)

#### GetCandidateProfileById
- ✅ Returns candidate profile with valid UserId
- ✅ Returns complete profile with all related data
- ✅ Throws exception with invalid UserId

#### AddCandidateProfile
- ✅ Returns success StatusDTO (200) for valid candidate
- ✅ Throws exception for duplicate UserId
- ✅ Successfully adds candidate with related entities

#### UpdateCandidateProfile
- ✅ Returns success StatusDTO (200) for valid update
- ✅ Throws exception for non-existent candidate
- ✅ Updates all candidate properties
- ✅ Properly calls repository method

#### DeleteCandidateProfile
- ✅ Returns success StatusDTO (200) for valid deletion
- ✅ Throws exception for non-existent candidate
- ✅ Calls repository delete method exactly once

#### Service Error Handling
- ✅ Handles ArgumentNullException
- ✅ All methods properly delegate to repository
- ✅ Consistent error propagation

### 3. Integration Tests (`CandidateIntegrationTests.cs`)

#### Complete CRUD Workflow
- ✅ Creates a candidate successfully
- ✅ Retrieves created candidate
- ✅ Updates candidate profile
- ✅ Verifies updated data
- ✅ Deletes candidate
- ✅ All operations return proper status codes

#### Data Consistency
- ✅ Multiple candidates managed independently
- ✅ Related entities maintained after updates
- ✅ Correct entity counts preserved

#### Error Scenarios
- ✅ Duplicate candidate detection
- ✅ Non-existent candidate updates
- ✅ Non-existent candidate deletion
- ✅ Non-existent candidate retrieval
- ✅ Appropriate error messages

#### Response Format
- ✅ StatusDTO always returns correct format
- ✅ All operations return consistent status code (200)
- ✅ All operations include status message

### 4. Edge Case Tests (`CandidateServiceEdgeCaseTests.cs`)

#### Null and Empty Input
- ✅ Null candidate throws ArgumentNullException
- ✅ Empty FirstName still accepted
- ✅ Empty Guid throws InvalidOperationException

#### User ID Uniqueness
- ✅ Duplicate UserId throws exception
- ✅ Multiple different UserIds processed successfully

#### Large Data Sets
- ✅ Update candidate with 10+ related entities
- ✅ Delete candidate with complex profiles
- ✅ Process large address/skill/experience lists

#### Data Type and Format
- ✅ Various email formats accepted
- ✅ Various phone number formats accepted
- ✅ Various date-of-birth formats accepted

#### Concurrency and Order
- ✅ Sequential operations maintain data integrity

#### Status Code Consistency
- ✅ All successful operations return status code 200
- ✅ Success messages are non-empty and meaningful

#### Builder Pattern
- ✅ CandidateBuilder creates valid candidates
- ✅ Builder method chaining works correctly

## Test Data Factory & Builder

### CandidateTestDataFactory

Provides factory methods for creating test data:

```csharp
// Simple valid candidate
var candidate = CandidateTestDataFactory.CreateValidCandidate();

// Candidate with complete profile
var fullCandidate = CandidateTestDataFactory.CreateCandidateWithCompleteProfile();

// Multiple candidates
var candidates = CandidateTestDataFactory.CreateMultipleCandidates(count: 5);

// Specific entity collections
var addresses = CandidateTestDataFactory.CreateAddresses(count: 3);
var languages = CandidateTestDataFactory.CreateLanguages(count: 5);
var educations = CandidateTestDataFactory.CreateEducations(count: 2);
var skills = CandidateTestDataFactory.CreateSkills(count: 10);
var experiences = CandidateTestDataFactory.CreateWorkExperiences(count: 3);
```

### CandidateBuilder

Fluent builder pattern for test candidate construction:

```csharp
var candidate = CandidateBuilder.Create()
	.WithFirstName("John")
	.WithLastName("Doe")
	.WithEmail("john@example.com")
	.WithGender(Gender.Male)
	.WithCompleteProfile()
	.Build();
```

## Running the Tests

### All Tests
```bash
dotnet test Candidate.Tests
```

### Specific Test Class
```bash
dotnet test Candidate.Tests --filter ClassName=CandidateRepositoryTests
```

### Specific Test Method
```bash
dotnet test Candidate.Tests --filter Name~GetCandidateProfileByIdAsync_WithValidUserId_ReturnsCandidateProfile
```

### With Code Coverage
```bash
dotnet test Candidate.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

### Verbose Output
```bash
dotnet test Candidate.Tests --logger "console;verbosity=detailed"
```

## Test Statistics

| Category | Count |
|----------|-------|
| Repository Tests | 13 |
| Service Tests | 16 |
| Integration Tests | 9 |
| Edge Case Tests | 23 |
| **Total Tests** | **61** |

## Mocking Strategy

Tests use **Moq** for dependency injection:

```csharp
private readonly Mock<ICandidateRepository> _mockRepository;

// Setup mock behavior
_mockRepository
	.Setup(x => x.GetCandidateProfileByIdAsync(userId))
	.ReturnsAsync(expectedCandidate);

// Verify method was called
_mockRepository.Verify(x => x.GetCandidateProfileByIdAsync(userId), Times.Once);
```

## Best Practices Used

1. **Arrange-Act-Assert (AAA)** - Clear test structure
2. **Descriptive Test Names** - Names explain what is being tested
3. **Single Responsibility** - Each test validates one behavior
4. **Mocking** - External dependencies are mocked
5. **Test Data Factory** - Reusable test data generation
6. **Builder Pattern** - Fluent test setup
7. **Theory Tests** - Parameterized tests for multiple scenarios
8. **Edge Cases** - Boundary conditions and error scenarios
9. **Integration Tests** - Full workflow testing
10. **Code Coverage** - Comprehensive test coverage

## Coverage Goals

Target coverage per component:
- **CandidateRepository**: 95%+
- **CandidateService**: 95%+
- **Overall**: 90%+

## Continuous Integration

These tests are designed to run in CI/CD pipelines:

```yaml
# Example GitHub Actions
- name: Run Tests
  run: dotnet test Candidate.Tests --logger "console;verbosity=normal"

- name: Generate Coverage
  run: dotnet test Candidate.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## Troubleshooting

### Tests Failing Due to Missing Dependencies
- Ensure all NuGet packages are restored: `dotnet restore`

### Mock Setup Issues
- Verify mock return types match expected method signatures
- Check that async methods return `Task<T>` or `Task`

### Timing Issues
- Use `ReturnsAsync()` for async mock setups
- Ensure `await` is used for async test methods

## Contributing

When adding new tests:

1. Follow existing naming conventions
2. Use AAA pattern consistently
3. Add reusable test data to `CandidateTestDataFactory`
4. Include positive and negative test cases
5. Update this README with new test categories
6. Aim for >90% code coverage

## References

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

**Last Updated**: 2024
**Maintainer**: Development Team
