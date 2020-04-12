using System.Collections.Generic;
using FluentAssertions;
using src.Business;
using src.Repository;
using tests.Resources;
using Xunit;

namespace tests.Unit.Business
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
                .FilledAllColumnsUntilColumn(2)
                .build();

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
                .FilledAllColumnsUntilColumn(1)
                .build();

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
                .FilledAllColumnsUntilColumn(2)
                .build();

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
                .FilledAllColumnsUntilColumn(2)
                .build();

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
                .FilledAllColumnsUntilColumn(1)
                .build();

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
                .FilledAllColumnsUntilColumn(2)
                .build();

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
                .GivenColumnAndRow(0, 0)
                .FilledAllRowsUntilRows(2)
                .build();

            var wonVertically = _boardJudge.WonVertically(board, 1);

            wonVertically.Should().BeTrue();
        }
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column1()
        {
            throw new System.NotImplementedException();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row0Column2()
        {
            throw new System.NotImplementedException();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllVerticalFieldsScenarioBoard3X3Row2Column1()
        {
            throw new System.NotImplementedException();
        } 
        
        [Fact]
        public void ShouldReturnTrueGivenThePlayerIsPresentInAllDiagonalFieldsScenarioBoard3X3Row0Column0()
        {
            throw new System.NotImplementedException();
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
