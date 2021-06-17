using System.Linq;
using FluentAssertions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Business
{
    public class BoardDealerTest
    {
        private readonly IBoardDealer _boardDealer = new BoardDealer(new BoardJudge());

        [Fact(DisplayName = "When given board size is not supported")]
        public void ShouldReturnFalseIfBoardSizeDoesNotMatchRegex()
        {
            _boardDealer.NotValidOrUnsupportedBoardSize("Cockatiel").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("3").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("1v1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2v2").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("3v3").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("4x3").Should().BeTrue();
        }

        [Fact(DisplayName = "When given board size is supported")]
        public void ShouldReturnTrueIfBoardSizeHasColumnAndRowsGreaterThan2AndAreEqualAndLessThan10()
        {
            _boardDealer.NotValidOrUnsupportedBoardSize("0x0").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("1x1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2x2").Should().BeTrue();
            // Minimum is 3x3
            _boardDealer.NotValidOrUnsupportedBoardSize("3x3").Should().BeFalse();
            _boardDealer.NotValidOrUnsupportedBoardSize("4x4").Should().BeFalse();
            _boardDealer.NotValidOrUnsupportedBoardSize("9x9").Should().BeFalse();
            // Maximum is 9x9
            _boardDealer.NotValidOrUnsupportedBoardSize("10x10").Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateRequestedBoard()
        {
            // Arrange
            var boardSetup = new { columnSize = 4, rowSize = 3 };
            var boardSize = $"{boardSetup.columnSize}x{boardSetup.rowSize}";
            var playerOne = new Player { Name = "Bear" };
            var playerTwo = new Player { Name = "Salted Man" };
            // Act
            var board = _boardDealer.PrepareBoardWithRequestSetup(boardSize, playerOne, playerTwo);
            // Assert
            board.NumberOfColumn.Should().Be(boardSetup.columnSize);
            board.NumberOfRows.Should().Be(boardSetup.rowSize);
            board.PlayerBoards.Should().HaveCount(2);
            board.FieldsConfiguration.Should().NotBeNull();
            board.FreeFields.Should().NotBeNull();
            var expectedPlayerOne = board.PlayerBoards.First().Player;
            var expectedPlayerTwo = board.PlayerBoards.Last().Player;
            expectedPlayerOne.Should().Be(playerOne);
            expectedPlayerTwo.Should().Be(playerTwo);
        }

        [Fact]
        public void ShouldCheckIfGivenMovementFinishesTheGame()
        {
            // Arrange
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(new Player { Name = "Iago" })
                .GivenRow(0)
                .FillAllColumnsUntilColumn(2)
                .Build();
            int position = 3;
            // Act
            var boardState = _boardDealer.EvaluateTheSituation(board, position);
            // Assert
            boardState.HasAWinner.Should().BeTrue();
        }

        [Fact]
        public void ShouldCreateMovementForPlayer()
        {
            // Arrange
            var player = new Player { Name = "Iago" };
            var board = new BoardBuilder().BoardSize(3).Build();
            var freeFieldsCountDuringArrange = board.FreeFields.Count;
            var position = 1;
            // Act
            var movement = _boardDealer.CreateMovementForCustomPlayerOrComputer(board, position, player);
            // Assert
            board.FreeFields.Count.Should().Be(freeFieldsCountDuringArrange - 1);
            board.FieldsConfiguration[0][0].Should().Be(player);
            movement.Position.Should().Be(position);
            movement.WhoMade.Should().Be(player);
        }

        [Fact]
        public void ShouldCreateMovementForRobotPlayer()
        {
            // Arrange
            var playerIago = new Player { Name = "Iago", Computer = false };
            var playerRose = new Player { Name = "Rose", Computer = true };
            var board = new BoardBuilder().BoardSize(3).AddPlayers(playerIago, playerRose).Build();
            var freeFieldsCountDuringArrange = board.FreeFields.Count;
            var position = 1;
            // Act
            var movement = _boardDealer.CreateMovementForCustomPlayerOrComputer(board, position);
            // Assert
            board.FreeFields.Count.Should().Be(freeFieldsCountDuringArrange - 1);
            board.FieldsConfiguration[0][0].Should().Be(playerRose);
            movement.Position.Should().Be(position);
            movement.WhoMade.Should().Be(playerRose);
        }
    }
}
