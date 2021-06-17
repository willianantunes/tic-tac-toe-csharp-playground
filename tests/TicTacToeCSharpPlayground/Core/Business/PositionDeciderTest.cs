using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TicTacToeCSharpPlayground.Core.Business;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Business
{
    public class PositionDeciderTest
    {
        private readonly IPositionDecider _positionDecider = new PositionDecider();

        [Theory(DisplayName = "Should retrieve random position given a list of a range of available ones")]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        [InlineData(1, 4)]
        [InlineData(1, 5)]
        [InlineData(1, 6)]
        [InlineData(1, 7)]
        [InlineData(1, 8)]
        [InlineData(1, 9)]
        public void TestScenarioOne(int start, int count)
        {
            // Arrange
            IEnumerable<int> positions = Enumerable.Range(start, count);
            // Act
            var position = _positionDecider.ChooseTheBestAvailablePositionFor(positions);
            // Assert
            position.Should().BeInRange(start, count);
        }

        [Theory(DisplayName = "Should retrieve random position given list of available ones")]
        [InlineData(new[] { 2, 4, 6, 8 })]
        [InlineData(new[] { 1, 5, 8, 9 })]
        [InlineData(new[] { 1, 2, 8, 9 })]
        [InlineData(new[] { 1, 7, 8, 9 })]
        [InlineData(new[] { 3, 7, 8, 9 })]
        public void TestScenarioTwo(int[] availablePositions)
        {
            // Act
            var position = _positionDecider.ChooseTheBestAvailablePositionFor(availablePositions);
            // Assert
            position.Should().BeOneOf(availablePositions);
        }
    }
}
