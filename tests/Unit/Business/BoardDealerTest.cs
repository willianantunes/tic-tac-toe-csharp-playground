using FluentAssertions;
using src.Business;
using Xunit;

namespace tests.Unit.Business
{
    public class BoardDealerTest
    {
        [Fact]
        public void ShouldReturnFalseIfBoardSizeIsNull()
        {
            var boardDealer = new BoardDealer();

            boardDealer.NotValidOrUnsupportedBoardSize(null).Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfBoardSizeDoesNotMatchRegex()
        {
            var boardDealer = new BoardDealer();

            boardDealer.NotValidOrUnsupportedBoardSize("Calopsita").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("1").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("2").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("3").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("1v1").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("2v2").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("3v3").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("4x3").Should().BeTrue();
        }
        
        [Fact]
        public void ShouldReturnTrueIfBoardSizeHasColumnAndRowsGreaterThan2AndAreEqualAndLessThan10()
        {
            var boardDealer = new BoardDealer();

            // TODO: A loop can be used here
            boardDealer.NotValidOrUnsupportedBoardSize("1x1").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("2x2").Should().BeTrue();
            boardDealer.NotValidOrUnsupportedBoardSize("3x3").Should().BeFalse();
            boardDealer.NotValidOrUnsupportedBoardSize("4x4").Should().BeFalse();
            boardDealer.NotValidOrUnsupportedBoardSize("9x9").Should().BeFalse();
            boardDealer.NotValidOrUnsupportedBoardSize("10x10").Should().BeTrue();
        }
    }
}