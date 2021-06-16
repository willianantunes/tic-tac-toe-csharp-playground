using System.Collections.Generic;
using FluentAssertions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Business
{
    public class BoardJudgeTest
    {
        private readonly IBoardJudge _boardJudge = new BoardJudge();

        [Fact(DisplayName = "Should retrieve in which row and column the given position is")]
        public void ShouldReturnRowAndColumnGivenSpecificPositionForBoard3X3()
        {
            // Arrange
            var board = new Board { NumberOfColumn = 3, NumberOfRows = 3 };
            // Act
            var (row, col) = _boardJudge.GetRowAndColGivenAPosition(1, board);
            // Assert
            row.Should().Be(0);
            col.Should().Be(0);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(2, board);
            // Assert
            row.Should().Be(0);
            col.Should().Be(1);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(3, board);
            row.Should().Be(0);
            col.Should().Be(2);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(4, board);
            row.Should().Be(1);
            col.Should().Be(0);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(5, board);
            // Assert
            row.Should().Be(1);
            col.Should().Be(1);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(6, board);
            // Assert
            row.Should().Be(1);
            col.Should().Be(2);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(7, board);
            // Assert
            row.Should().Be(2);
            col.Should().Be(0);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(8, board);
            // Assert
            row.Should().Be(2);
            col.Should().Be(1);
            // Act
            (row, col) = _boardJudge.GetRowAndColGivenAPosition(9, board);
            // Assert
            row.Should().Be(2);
            col.Should().Be(2);
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var iago = new Player { Name = "Iago" };
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(iago)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(2)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 1);
            // Assert
            wonDiagonally.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var iago = new Player { Name = "Iago" };
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(iago)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(1)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 1);
            // Assert
            wonDiagonally.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column2()
        {
            // Arrange
            var jasmine = new Player { Name = "Jasmine" };
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(jasmine)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(2)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 3);
            // Assert
            wonDiagonally.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row1Column1()
        {
            // Arrange
            var saltedGuy = new Player { Name = "Salted Guy" };
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(saltedGuy)
                .GivenRow(1)
                .FillAllColumnsUntilColumn(2)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 5);
            // Assert
            wonDiagonally.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllHorizontalFieldsScenarioBoard3X3Row1Column1()
        {
            // Arrange
            var saltedGuy = new Player { Name = "Salted Guy" };
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(saltedGuy)
                .GivenRow(1)
                .FillAllColumnsUntilColumn(1)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 5);
            // Assert
            wonDiagonally.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row2Column0()
        {
            // Arrange
            var salParadise = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(salParadise)
                .GivenRow(2)
                .FillAllColumnsUntilColumn(2)
                .Build();
            // Act
            var wonDiagonally = _boardJudge.WonHorizontally(board, 7);
            // Assert
            wonDiagonally.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var gandalf = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 0)
                .FillAllRowsUntilRows(2)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonVertically(board, 1);
            // Assert
            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllVerticalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var manwe = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(manwe)
                .GivenRowAndColumn(0, 0)
                .FillAllRowsUntilRows(1)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonVertically(board, 1);
            // Assert
            wonVertically.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column1()
        {
            // Arrange
            var gandalf = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 1)
                .FillAllRowsUntilRows(2)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonVertically(board, 2);
            // Assert
            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column2()
        {
            // Arrange
            var gandalf = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 2)
                .FillAllRowsUntilRows(2)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonVertically(board, 3);
            // Assert
            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row2Column1()
        {
            // Arrange
            var gandalf = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 1)
                .FillAllRowsUntilRows(2)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonVertically(board, 8);
            // Assert
            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllDiagonalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var faramir = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(faramir)
                .GivenRowAndColumn(0, 0)
                .FillDiagonallyUntilRow(2)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonDiagonally(board, 1);
            // Assert
            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllDiagonalFieldsScenarioBoard3X3Row0Column0()
        {
            // Arrange
            var faramir = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(faramir)
                .GivenRowAndColumn(0, 0)
                .FillDiagonallyUntilRow(1)
                .Build();
            // Act
            var wonVertically = _boardJudge.WonDiagonally(board, 1);
            // Assert
            wonVertically.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllReverseDiagonalFieldsScenarioBoard3X3Row0Column2()
        {
            // Arrange
            var boromir = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(boromir)
                .GivenRowAndColumn(0, 2)
                .FillReverseDiagonallyUntilRow(2)
                .Build();
            // Act
            var wonReverseVertically = _boardJudge.WonReverseDiagonally(board, 3);
            // Assert
            wonReverseVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllReverseDiagonalFieldsScenarioBoard3X3Row0Column2()
        {
            // Arrange
            var boromir = new Player();
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(boromir)
                .GivenRowAndColumn(0, 2)
                .FillReverseDiagonallyUntilRow(1)
                .Build();
            // Act
            var wonReverseVertically = _boardJudge.WonReverseDiagonally(board, 3);
            // Assert
            wonReverseVertically.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnFalseGivenNotAllFieldsAreFilled()
        {
            // 1: Arrange
            var columnsRowOne = new List<Player> { null, null, null };
            var columnsRowTwo = new List<Player> { null, null, null };
            var columnsRowThree = new List<Player> { null, null, null };
            var rows = new List<IList<Player>> { columnsRowOne, columnsRowTwo, columnsRowThree };
            // 1: Act        
            var drawGameOne = _boardJudge.DrawGame(rows);
            // 1: Assert
            drawGameOne.Should().BeFalse();
            // 2: Arrange
            var jafar = new Player();
            columnsRowOne = new List<Player> { jafar, jafar, jafar };
            columnsRowTwo = new List<Player> { jafar, jafar, jafar };
            columnsRowThree = new List<Player> { jafar, jafar, null };
            rows = new List<IList<Player>> { columnsRowOne, columnsRowTwo, columnsRowThree };
            // 2: Assert
            var drawGameTwo = _boardJudge.DrawGame(rows);
            drawGameTwo.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenAllFieldsAreFilled()
        {
            // Arrange
            var aladdin = new Player();
            var columnsRowOne = new List<Player> { aladdin, aladdin, aladdin };
            var columnsRowTwo = new List<Player> { aladdin, aladdin, aladdin };
            var columnsRowThree = new List<Player> { aladdin, aladdin, aladdin };
            var rows = new List<IList<Player>> { columnsRowOne, columnsRowTwo, columnsRowThree };
            // Act
            var drawGame = _boardJudge.DrawGame(rows);
            // Assert
            drawGame.Should().BeTrue();
        }
    }
}
