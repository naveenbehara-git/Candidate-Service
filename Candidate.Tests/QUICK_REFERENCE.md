# Candidate.Tests - Quick Reference Guide

## 🚀 Quick Start

### Running Tests
```bash
# All tests
dotnet test Candidate.Tests

# Specific test class
dotnet test Candidate.Tests --filter CandidateRepositoryTests

# Specific test method
dotnet test Candidate.Tests --filter "GetCandidateProfileByIdAsync_WithValidUserId"

# With details
dotnet test Candidate.Tests --verbosity detailed

# With code coverage
dotnet test Candidate.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

## 📂 File Organization

| File | Purpose | Tests |
|------|---------|-------|
| `CandidateRepositoryTests.cs` | Repository layer tests | 13 |
| `CandidateServiceTests.cs` | Service layer tests | 16 |
| `CandidateIntegrationTests.cs` | End-to-end workflows | 9 |
| `CandidateServiceEdgeCaseTests.cs` | Edge cases & boundaries | 23 |
| `CandidateTestDataFactory.cs` | Test data generation | Utilities |
| `TestSetup.cs` | Global configuration | Utilities |

## 🧪 Test Categories at a Glance

### Repository Tests (13)
```
✓ GetCandidateProfileByIdAsync (3)
  - Valid UserId
  - Invalid UserId
  - Load all related entities

✓ InsertCandidate (3)
  - Valid insertion
  - Duplicate UserId
  - Related entities

✓ UpdateCandidateAsync (3)
  - Valid update
  - Non-existent candidate
  - Timestamp update

✓ DeleteCandidateAsync (4)
  - Valid deletion
  - Invalid UserId
  - All related entities deleted
  - Cascade delete verification
```

### Service Tests (16)
```
✓ GetCandidateProfileById (3)
✓ AddCandidateProfile (3)
✓ UpdateCandidateProfile (3)
✓ DeleteCandidateProfile (3)
✓ Error Handling (4)
```

### Integration Tests (9)
```
✓ Complete CRUD Workflow (1)
✓ Data Consistency (2)
✓ Error Scenarios (4)
✓ Response Format (2)
```

### Edge Cases (23)
```
✓ Null & Empty Input (3)
✓ User ID Uniqueness (2)
✓ Large Data Sets (3)
✓ Data Formats (3)
✓ Concurrency (1)
✓ Status Codes (2)
✓ Builder Pattern (1)
```

## 💡 Using Test Utilities

### Create Test Data

**Simple candidate:**
```csharp
var candidate = CandidateTestDataFactory.CreateValidCandidate();
```

**Complete profile:**
```csharp
var candidate = CandidateTestDataFactory.CreateCandidateWithCompleteProfile();
```

**Using builder:**
```csharp
var candidate = CandidateBuilder.Create()
	.WithFirstName("John")
	.WithLastName("Doe")
	.WithCompleteProfile()
	.Build();
```

### Assertions

**Basic equality:**
```csharp
Assert.Equal("John", candidate.FirstName);
```

**Collection assertions:**
```csharp
Assert.NotEmpty(candidate.Languages);
Assert.Equal(3, candidate.Languages.Count);
```

**Custom assertions:**
```csharp
CandidateAssertions.AssertCandidateEqual(expected, actual);
CandidateAssertions.AssertCandidateHasAllRelatedEntities(candidate);
```

## 🎯 Mock Setup Patterns

### Basic Mock Setup
```csharp
var mock = new Mock<ICandidateRepository>();
mock.Setup(x => x.GetCandidateProfileByIdAsync(userId))
	.ReturnsAsync(candidate);
```

### Mock Verification
```csharp
mock.Verify(x => x.InsertCandidate(candidate), Times.Once);
mock.Verify(x => x.DeleteCandidateAsync(userId), Times.Never);
```

### Mock with Any Parameters
```csharp
mock.Setup(x => x.InsertCandidate(It.IsAny<Candidate>()))
	.ReturnsAsync((Candidate c) => c);
```

## 📋 Test Template

Use this template for new tests:

```csharp
[Fact]
public async Task MethodName_Condition_ExpectedResult()
{
	// Arrange
	var input = CandidateTestDataFactory.CreateValidCandidate();
	_mockRepository
		.Setup(x => x.SomeMethod(input))
		.ReturnsAsync(input);

	// Act
	var result = await _service.SomeMethod(input);

	// Assert
	Assert.NotNull(result);
	Assert.Equal("expected", result.Property);
	_mockRepository.Verify(x => x.SomeMethod(input), Times.Once);
}
```

## 🔧 Common Operations

### Testing CRUD Operations

**Create:**
```csharp
[Fact]
public async Task AddCandidateProfile_WithValidCandidate_ReturnsSuccess()
{
	// Arrange
	var candidate = CandidateTestDataFactory.CreateValidCandidate();
	_mockRepository.Setup(x => x.InsertCandidate(candidate))
		.ReturnsAsync(candidate);

	// Act
	var result = await _service.AddCandidateProfile(candidate);

	// Assert
	Assert.Equal(200, result.StatusCode);
}
```

**Read:**
```csharp
[Fact]
public async Task GetCandidateProfileById_WithValidUserId_ReturnsCandidateProfile()
{
	// Arrange
	var userId = Guid.NewGuid();
	var candidate = CandidateTestDataFactory.CreateValidCandidate(userId: userId);
	_mockRepository.Setup(x => x.GetCandidateProfileByIdAsync(userId))
		.ReturnsAsync(candidate);

	// Act
	var result = await _service.GetCandidateProfileById(userId);

	// Assert
	Assert.NotNull(result);
	Assert.Equal(userId, result.UserId);
}
```

**Update:**
```csharp
[Fact]
public async Task UpdateCandidateProfile_WithValidCandidate_ReturnsSuccess()
{
	// Arrange
	var candidate = CandidateTestDataFactory.CreateValidCandidate();
	candidate.CandidateId = 1;
	_mockRepository.Setup(x => x.UpdateCandidateAsync(candidate))
		.ReturnsAsync(candidate);

	// Act
	var result = await _service.UpdateCandidateProfile(candidate);

	// Assert
	Assert.Equal(200, result.StatusCode);
}
```

**Delete:**
```csharp
[Fact]
public async Task DeleteCandidateProfile_WithValidUserId_ReturnsSuccess()
{
	// Arrange
	var userId = Guid.NewGuid();
	_mockRepository.Setup(x => x.DeleteCandidateAsync(userId))
		.ReturnsAsync(true);

	// Act
	var result = await _service.DeleteCandidateProfile(userId);

	// Assert
	Assert.Equal(200, result.StatusCode);
}
```

## 🎓 Test Constants

```csharp
// Use predefined constants for consistency
TestDataConstants.VALID_FIRST_NAME        // "John"
TestDataConstants.VALID_LAST_NAME         // "Doe"
TestDataConstants.VALID_EMAIL             // "john.doe@example.com"
TestDataConstants.VALID_PHONE             // "1234567890"
TestDataConstants.VALID_CITY              // "New York"
TestDataConstants.VALID_COUNTRY           // "USA"
TestDataConstants.VALID_INSTITUTION       // "MIT"
TestDataConstants.VALID_SKILL_NAME        // "C#"
TestDataConstants.VALID_COMPANY           // "Google"
TestDataConstants.VALID_LANGUAGE          // "English"
TestDataConstants.VALID_MARITAL_STATUS    // "Single"
TestDataConstants.VALID_USER_ID           // Guid.Parse("00000001-...")
TestDataConstants.VALID_DOB               // "1990-01-01"
```

## ❌ Common Mistakes to Avoid

1. **Forgetting async/await**
   ```csharp
   // ❌ Wrong
   public void TestMethod() { }

   // ✅ Correct
   public async Task TestMethod() { }
   ```

2. **Not setting up mocks**
   ```csharp
   // ❌ Missing Setup
   var result = await _service.GetCandidateProfileById(userId);

   // ✅ With Setup
   _mockRepository.Setup(x => x.GetCandidateProfileByIdAsync(userId))
	   .ReturnsAsync(candidate);
   var result = await _service.GetCandidateProfileById(userId);
   ```

3. **Using ReturnsAsync for synchronous methods**
   ```csharp
   // ❌ Wrong
   _mockRepository.Setup(x => x.SyncMethod())
	   .ReturnsAsync(result);

   // ✅ Correct
   _mockRepository.Setup(x => x.SyncMethod())
	   .Returns(result);
   ```

4. **Not verifying mocks**
   ```csharp
   // ❌ Missing verification
   var result = await _service.AddCandidateProfile(candidate);
   Assert.Equal(200, result.StatusCode);

   // ✅ With verification
   var result = await _service.AddCandidateProfile(candidate);
   Assert.Equal(200, result.StatusCode);
   _mockRepository.Verify(x => x.InsertCandidate(candidate), Times.Once);
   ```

## 📊 Test Metrics

| Metric | Value |
|--------|-------|
| Total Tests | 61 |
| Repository Tests | 13 |
| Service Tests | 16 |
| Integration Tests | 9 |
| Edge Case Tests | 23 |
| Code Coverage Target | 90%+ |
| Framework | xUnit 2.6.2 |
| Mock Library | Moq 4.20.70 |

## 🔗 Related Files

- `Candidate.Application/Services/CandidateService.cs` - Service implementation
- `Candidate.Infrastructure/Repositories/CandidateRepository.cs` - Repository implementation
- `Candidate.Domain/Model/Candidate.cs` - Entity model
- `README.md` - Detailed documentation

## 💾 File Locations

```
Candidate.Tests/
├── Repositories/CandidateRepositoryTests.cs
├── Services/CandidateServiceTests.cs
├── Integration/CandidateIntegrationTests.cs
├── EdgeCases/CandidateServiceEdgeCaseTests.cs
├── Fixtures/CandidateTestDataFactory.cs
├── TestSetup.cs
├── Candidate.Tests.csproj
├── README.md
└── IMPLEMENTATION_SUMMARY.md
```

## 🆘 Troubleshooting

| Issue | Solution |
|-------|----------|
| Tests not running | Run `dotnet restore` then `dotnet test` |
| Mock setup errors | Ensure return types match (Task vs ValueTask) |
| Null reference exceptions | Check mock Setup() is called before using |
| Timeout errors | Increase test timeout in TestConfiguration |
| Parallel execution issues | Check if test marked with `[Collection]` |

## 📞 Quick Links

- **Run All Tests**: `dotnet test Candidate.Tests`
- **Run with Coverage**: `dotnet test Candidate.Tests /p:CollectCoverage=true`
- **View Test Explorer**: Open Test Explorer in Visual Studio
- **Read Docs**: See `README.md` for detailed documentation

---

**Last Updated**: 2024
**Framework**: xUnit 2.6.2
**Tests**: 61 total
