using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit3TestReport.Tests
{
    [TestFixture]
    public class TestRunTests
    {
        [Test]
        public void ToHtml_ReturnsErrorMessageAndFilename_IfTestResultData_IsNotValid()
        {
            // Arrange
            var testResultData = new TestRun
            {
                IsValid = false,
                FileName = "invalid.xml"
            };

            // Act
            var result = testResultData.ToHtml();

            // Assert
            Assert.That(result, Contains.Substring("File could not be parsed"));
            Assert.That(result, Contains.Substring("invalid.xml"));
        }

        [Test]
        public void ToHtml_ReturnsFailedTestDetails_IfContainsFailedTests()
        {
            // Arrange
            var testResultData = new TestRun
            {
                IsValid = true,
                FileName = "failed.xml",
                Result = "Failed",
                Total = 1,
                Failed = 1,
                Inconclusive = 0,
                Passed = 0,
                Skipped = 0,
                Duration = 0.45
            };

            // Act
            var result = testResultData.ToHtml();

            // Assert
            Assert.That(result, Contains.Substring("Failed"));
        }

        [Test]
        public void ToHtml_ReturnsTestDetail_WhenDoesNotContainFailedTests()
        {
            // Arrange
            var testResultData = new TestRun
            {
                IsValid = true,
                FileName = "success.xml",
                Result = "Passed",
                Total = 10,
                Failed = 0,
                Inconclusive = 1,
                Passed = 7,
                Skipped = 2,
                Duration = 2.45
            };

            // Act
            var result = testResultData.ToHtml();

            // Assert
            Assert.That(result, Contains.Substring("success.xml"));
            Assert.That(result, Contains.Substring("Passed"));
        }
    }
}
