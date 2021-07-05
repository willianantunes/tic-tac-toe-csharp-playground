using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Tests.Support;
using Xunit;

namespace Tests.PerformanceTesting
{
    public class PerformanceTestingResultTest
    {
        private JObject _generalStatistics;
        private JObject _applicationPerformanceIndex;

        [SkippableFact(DisplayName = "Should have no error in all requests")]
        public void TestScenario1()
        {
            // Arrange
            PreparePropertiesAndSkipTestIfNeeded();
            // Assert only
            _generalStatistics.Count.Should().Be(11);

            foreach (var token in _generalStatistics)
            {
                var errorCount = token.Value["errorCount"].As<int>();
                errorCount.Should().Be(0);
            }
        }

        [SkippableFact(DisplayName = "Should APDEX result be greater than 0.9 to all evaluated requests")]
        public void TestScenario2()
        {
            // Arrange
            PreparePropertiesAndSkipTestIfNeeded();
            var rawOverallResult = _applicationPerformanceIndex["overall"]["data"];
            var rawRequestsResults = _applicationPerformanceIndex["items"];
            // Assert
            rawOverallResult.Should().NotBeNullOrEmpty();
            rawRequestsResults.Count().Should().Be(10);

            foreach (var token in rawRequestsResults)
            {
                var apdexResultForGivenRequest = token["data"][0].Value<double>();
                apdexResultForGivenRequest.Should().BeGreaterThan(0.9);
            }
        }

        private void PreparePropertiesAndSkipTestIfNeeded()
        {
            // Skip test if required
            var value = Environment.GetEnvironmentVariable("EVALUATE_PERFORMANCE_TESTING");
            bool.TryParse(value, out var shouldNotSkipTest);
            Skip.If(shouldNotSkipTest is false, "It was informed to be skipped");
            // General statistics
            _generalStatistics = FileHandler.ReadFileAsDictionary("tests-jmeter/statistics.json");
            // APDEX sadly is not in only one file, we should extract it
            var lineWhereApdexResultIs = @"createTable($(""#apdexTable"")";
            var regexPatternToCaptureResult = @", ({"".+]}), ";
            var dashBoardFile = FileHandler.EnumerableFromFile("tests-jmeter/content/js/dashboard.js");
            foreach (var line in dashBoardFile)
            {
                if (line.Contains(lineWhereApdexResultIs))
                {
                    var match = Regex.Match(line, regexPatternToCaptureResult);
                    var rawApdexResult = match.Groups[1].Value;
                    _applicationPerformanceIndex = JObject.Parse(rawApdexResult);
                }
            }
        }
    }
}