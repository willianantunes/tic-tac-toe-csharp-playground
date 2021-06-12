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
        
        [Fact]
        public void ShouldReturnRowAndColumnGivenSpecificPositionForBoard3X3()
        {
            var board = new Board();
            board.NumberOfColumn = 3;
            board.NumberOfRows = 3;
            var boardJudge = new BoardJudge();

            var (row, col) = boardJudge.GetRowAndColGivenAPosition(1, board);
            row.Should().Be(0);
            col.Should().Be(0);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(2, board);
            row.Should().Be(0);
            col.Should().Be(1);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(3, board);
            row.Should().Be(0);
            col.Should().Be(2);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(4, board);
            row.Should().Be(1);
            col.Should().Be(0);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(5, board);
            row.Should().Be(1);
            col.Should().Be(1);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(6, board);
            row.Should().Be(1);
            col.Should().Be(2);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(7, board);
            row.Should().Be(2);
            col.Should().Be(0);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(8, board);
            row.Should().Be(2);
            col.Should().Be(1);
            (row, col) = boardJudge.GetRowAndColGivenAPosition(9, board);
            row.Should().Be(2);
            col.Should().Be(2);
        }

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column0()
        {
            var iago = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(iago)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(2)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 1);
            
            wonDiagonally.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column0()
        {
            var iago = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(iago)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(1)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 1);
            
            wonDiagonally.Should().BeFalse();
        }
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row0Column2()
        {
            var jasmine = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(jasmine)
                .GivenRow(0)
                .FillAllColumnsUntilColumn(2)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 3);

            wonDiagonally.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row1Column1()
        {
            var saltedGuy = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(saltedGuy)
                .GivenRow(1)
                .FillAllColumnsUntilColumn(2)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 5);

            wonDiagonally.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllHorizontalFieldsScenarioBoard3X3Row1Column1()
        {
            var saltedGuy = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(saltedGuy)
                .GivenRow(1)
                .FillAllColumnsUntilColumn(1)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 5);

            wonDiagonally.Should().BeFalse();
        }         
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllHorizontalFieldsScenarioBoard3X3Row2Column0()
        {
            var salParadise = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(salParadise)
                .GivenRow(2)
                .FillAllColumnsUntilColumn(2)
                .Build();

            var wonDiagonally = _boardJudge.WonHorizontally(board, 7);

            wonDiagonally.Should().BeTrue();
        } 

        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column0()
        {
            var gandalf = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 0)
                .FillAllRowsUntilRows(2)
                .Build();

            var wonVertically = _boardJudge.WonVertically(board, 1);

            wonVertically.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllVerticalFieldsScenarioBoard3X3Row0Column0()
        {
            var manwe = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(manwe)
                .GivenRowAndColumn(0, 0)
                .FillAllRowsUntilRows(1)
                .Build();

            var wonVertically = _boardJudge.WonVertically(board, 1);

            wonVertically.Should().BeFalse();
        }
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column1()
        {
            var gandalf = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 1)
                .FillAllRowsUntilRows(2)
                .Build();

            var wonVertically = _boardJudge.WonVertically(board, 2);

            wonVertically.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column2()
        {
            var gandalf = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 2)
                .FillAllRowsUntilRows(2)
                .Build();

            var wonVertically = _boardJudge.WonVertically(board, 3);

            wonVertically.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row2Column1()
        {
            var gandalf = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(gandalf)
                .GivenRowAndColumn(0, 1)
                .FillAllRowsUntilRows(2)
                .Build();

            var wonVertically = _boardJudge.WonVertically(board, 8);

            wonVertically.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllDiagonalFieldsScenarioBoard3X3Row0Column0()
        {
            var faramir = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(faramir)
                .GivenRowAndColumn(0, 0)
                .FillDiagonallyUntilRow(2)
                .Build();

            var wonVertically = _boardJudge.WonDiagonally(board, 1);

            wonVertically.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllDiagonalFieldsScenarioBoard3X3Row0Column0()
        {
            var faramir = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(faramir)
                .GivenRowAndColumn(0, 0)
                .FillDiagonallyUntilRow(1)
                .Build();

            var wonVertically = _boardJudge.WonDiagonally(board, 1);

            wonVertically.Should().BeFalse();
        }  
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllReverseDiagonalFieldsScenarioBoard3X3Row0Column2()
        {
            var boromir = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(boromir)
                .GivenRowAndColumn(0, 2)
                .FillReverseDiagonallyUntilRow(2)
                .Build();

            var wonReverseVertically = _boardJudge.WonReverseDiagonally(board, 3);

            wonReverseVertically.Should().BeTrue();
        } 
        
        [Fact]
        public void ShouldReturnFalseGivenThePlayerIsNotPresentInAllReverseDiagonalFieldsScenarioBoard3X3Row0Column2()
        {
            var boromir = new Player();
            
            var board = new BoardBuilder()
                .BoardSize(3)
                .WithPlayer(boromir)
                .GivenRowAndColumn(0, 2)
                .FillReverseDiagonallyUntilRow(1)
                .Build();

            var wonReverseVertically = _boardJudge.WonReverseDiagonally(board, 3);

            wonReverseVertically.Should().BeFalse();
        } 
        
        [Fact]
        public void ShouldReturnFalseGivenNotAllFieldsAreFilled()
        {
            var boardJudge = new BoardJudge();

            var rows = new List<IList<Player?>>();
            var columnsRowOne = new List<Player?>();
            columnsRowOne.Add(null);
            columnsRowOne.Add(null);
            columnsRowOne.Add(null);
            var columnsRowTwo = new List<Player?>();
            columnsRowTwo.Add(null);
            columnsRowTwo.Add(null);
            columnsRowTwo.Add(null);
            var columnsRowThree = new List<Player?>();
            columnsRowThree.Add(null);
            columnsRowThree.Add(null);
            columnsRowThree.Add(null);
            rows.Add(columnsRowThree);

            boardJudge.DrawGame(rows).Should().BeFalse();
            
            var jafar = new Player();
            rows = new List<IList<Player?>>();
            columnsRowOne = new List<Player?>();
            columnsRowOne.Add(jafar);
            columnsRowOne.Add(jafar);
            columnsRowOne.Add(jafar);
            columnsRowTwo = new List<Player?>();
            columnsRowTwo.Add(jafar);
            columnsRowTwo.Add(jafar);
            columnsRowTwo.Add(jafar);
            columnsRowThree = new List<Player?>();
            columnsRowThree.Add(jafar);
            columnsRowThree.Add(jafar);
            columnsRowThree.Add(null);
            rows.Add(columnsRowThree);
            
            boardJudge.DrawGame(rows).Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueGivenAllFieldsAreFilled()
        {
            var boardJudge = new BoardJudge();

            var aladdin = new Player();
            var rows = new List<IList<Player?>>();
            var columnsRowOne = new List<Player?>();
            columnsRowOne.Add(aladdin);
            columnsRowOne.Add(aladdin);
            columnsRowOne.Add(aladdin);
            var columnsRowTwo = new List<Player?>();
            columnsRowTwo.Add(aladdin);
            columnsRowTwo.Add(aladdin);
            columnsRowTwo.Add(aladdin);
            var columnsRowThree = new List<Player?>();
            columnsRowThree.Add(aladdin);
            columnsRowThree.Add(aladdin);
            columnsRowThree.Add(aladdin);
            rows.Add(columnsRowThree);
            
            boardJudge.DrawGame(rows).Should().BeTrue();
        }
    }
}
