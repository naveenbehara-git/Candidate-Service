using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Candidate.Tests
{
    /// <summary>
    /// Global test collection definitions for xUnit.
    /// Allows for parallel or sequential test execution control.
    /// </summary>
    [CollectionDefinition("Candidate Repository Collection")]
    public class CandidateRepositoryCollection : ICollectionFixture<CandidateRepositoryFixture>
    {
        // This class has no code, and never will. It's just used to define the collection.
    }

    [CollectionDefinition("Candidate Service Collection")]
    public class CandidateServiceCollection : ICollectionFixture<CandidateServiceFixture>
    {
        // This class has no code, and never will. It's just used to define the collection.
    }

    [CollectionDefinition("Candidate Integration Collection", DisableParallelization = true)]
    public class CandidateIntegrationCollection : ICollectionFixture<CandidateIntegrationFixture>
    {
        // This class has no code, and never will. It's just used to define the collection.
    }

    /// <summary>
    /// Fixture for repository-level tests.
    /// Sets up common test infrastructure for all repository tests.
    /// </summary>
    public class CandidateRepositoryFixture : IDisposable
    {
        public void Dispose()
        {
            // Cleanup after tests if needed
        }

        /// <summary>
        /// Gets test configuration for repository tests.
        /// </summary>
        public TestConfiguration GetConfiguration()
        {
            return new TestConfiguration
            {
                TimeoutMs = 5000,
                RetryCount = 1,
                ParallelExecutionEnabled = true
            };
        }
    }

    /// <summary>
    /// Fixture for service-level tests.
    /// Sets up common test infrastructure for all service tests.
    /// </summary>
    public class CandidateServiceFixture : IDisposable
    {
        public void Dispose()
        {
            // Cleanup after tests if needed
        }

        /// <summary>
        /// Gets test configuration for service tests.
        /// </summary>
        public TestConfiguration GetConfiguration()
        {
            return new TestConfiguration
            {
                TimeoutMs = 3000,
                RetryCount = 0,
                ParallelExecutionEnabled = true
            };
        }
    }

    /// <summary>
    /// Fixture for integration tests.
    /// Sets up common test infrastructure for integration tests.
    /// Note: Disables parallel execution to prevent database state issues.
    /// </summary>
    public class CandidateIntegrationFixture : IDisposable
    {
        public void Dispose()
        {
            // Cleanup after integration tests
        }

        /// <summary>
        /// Gets test configuration for integration tests.
        /// </summary>
        public TestConfiguration GetConfiguration()
        {
            return new TestConfiguration
            {
                TimeoutMs = 10000,
                RetryCount = 0,
                ParallelExecutionEnabled = false
            };
        }
    }

    /// <summary>
    /// Configuration settings for tests.
    /// </summary>
    public class TestConfiguration
    {
        /// <summary>
        /// Test timeout in milliseconds.
        /// </summary>
        public int TimeoutMs { get; set; }

        /// <summary>
        /// Number of times to retry a failed test.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Whether parallel execution is allowed.
        /// </summary>
        public bool ParallelExecutionEnabled { get; set; }
    }

    /// <summary>
    /// Test utilities for common operations.
    /// </summary>
    public static class TestUtilities
    {
        /// <summary>
        /// Retry a test operation with exponential backoff.
        /// Useful for tests with timing-dependent behavior.
        /// </summary>
        public static async System.Threading.Tasks.Task RetryAsync(
            Func<System.Threading.Tasks.Task> operation,
            int maxRetries = 3,
            int initialDelayMs = 100)
        {
            int delay = initialDelayMs;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch when (i < maxRetries - 1)
                {
                    await System.Threading.Tasks.Task.Delay(delay);
                    delay *= 2;
                }
            }

            // Final attempt without catching exception
            await operation();
        }

        /// <summary>
        /// Assert that a collection contains items with specific property values.
        /// </summary>
        public static void AssertCollectionContains<T>(
            IEnumerable<T> collection,
            Func<T, bool> predicate,
            string message = "Collection does not contain expected item")
        {
            Assert.NotNull(collection);
            Assert.True(collection.Any(predicate), message);
        }

        /// <summary>
        /// Assert that two collections contain the same items (regardless of order).
        /// </summary>
        public static void AssertCollectionsEqual<T>(
            IEnumerable<T> expected,
            IEnumerable<T> actual,
            Func<T, T, bool> comparer = null)
        {
            var expectedList = expected?.ToList() ?? new List<T>();
            var actualList = actual?.ToList() ?? new List<T>();

            Assert.Equal(expectedList.Count, actualList.Count);

            if (comparer != null)
            {
                foreach (var expectedItem in expectedList)
                {
                    Assert.Contains(expectedItem, actualList);
                }
            }
            else
            {
                // Use default equality
                foreach (var expectedItem in expectedList)
                {
                    Assert.Contains(expectedItem, actualList);
                }
            }
        }

        /// <summary>
        /// Get current test context information.
        /// </summary>
        public static string GetTestContextInfo()
        {
            return $"Test run at: {DateTime.UtcNow:O}";
        }
    }

    /// <summary>
    /// Custom assertion helpers for Candidate entities.
    /// </summary>
    public static class CandidateAssertions
    {
        public static void AssertCandidateEqual(
            Domain.Model.Candidate expected,
            Domain.Model.Candidate actual,
            bool ignoreDates = true)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
            Assert.Equal(expected.Gender, actual.Gender);
            Assert.Equal(expected.MaritalStatus, actual.MaritalStatus);

            if (!ignoreDates)
            {
                Assert.Equal(expected.DOB, actual.DOB);
                Assert.Equal(expected.CreatedAt, actual.CreatedAt);
            }
        }

        public static void AssertCandidateHasAllRelatedEntities(Domain.Model.Candidate candidate)
        {
            Assert.NotNull(candidate.Address);
            Assert.NotNull(candidate.Languages);
            Assert.NotNull(candidate.Educations);
            Assert.NotNull(candidate.Skills);
            Assert.NotNull(candidate.Experiences);

            Assert.NotEmpty(candidate.Address);
            Assert.NotEmpty(candidate.Languages);
            Assert.NotEmpty(candidate.Educations);
            Assert.NotEmpty(candidate.Skills);
            Assert.NotEmpty(candidate.Experiences);
        }

        public static void AssertCandidateIsValid(Domain.Model.Candidate candidate)
        {
            Assert.NotNull(candidate);
            Assert.NotNull(candidate.FirstName);
            Assert.NotNull(candidate.LastName);
            Assert.NotEqual(Guid.Empty, candidate.UserId);
            Assert.True(candidate.CandidateId > 0 || candidate.CandidateId == 0 && candidate.FirstName != null);
        }
    }

    /// <summary>
    /// Test data constants for use across test suite.
    /// </summary>
    public static class TestDataConstants
    {
        // Valid test data
        public const string VALID_FIRST_NAME = "John";
        public const string VALID_LAST_NAME = "Doe";
        public const string VALID_EMAIL = "john.doe@example.com";
        public const string VALID_PHONE = "1234567890";
        public const string VALID_CITY = "New York";
        public const string VALID_COUNTRY = "USA";
        public const string VALID_INSTITUTION = "MIT";
        public const string VALID_SKILL_NAME = "C#";
        public const string VALID_COMPANY = "Google";
        public const string VALID_LANGUAGE = "English";
        public const string VALID_MARITAL_STATUS = "Single";

        // Edge case data
        public const string EMPTY_STRING = "";
        public const string WHITESPACE_STRING = "   ";
        public const string VERY_LONG_STRING = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
        public const string SPECIAL_CHARACTERS_STRING = "!@#$%^&*()_+-={}[]|:;<>?,./";

        // Common GUIDs for testing
        public static readonly Guid VALID_USER_ID = Guid.Parse("00000001-0001-0001-0001-000000000001");
        public static readonly Guid ALTERNATE_USER_ID = Guid.Parse("00000002-0002-0002-0002-000000000002");

        // DateTime constants
        public static readonly DateTime VALID_DOB = DateTime.Parse("1990-01-01");
        public static readonly DateTime ALTERNATE_DOB = DateTime.Parse("1985-06-15");
        public static readonly DateTime OLD_DATE = DateTime.Parse("1950-01-01");
        public static readonly DateTime FUTURE_DATE = DateTime.Now.AddYears(1);
    }
}
