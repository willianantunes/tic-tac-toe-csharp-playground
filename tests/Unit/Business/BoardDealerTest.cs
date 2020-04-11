using System.Collections.Generic;
using FluentAssertions;
using src.Business;
using src.Helper;
using src.Repository;
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

        [Fact]
        public void ShouldInitializeBoardConfigurationGivenItsSetupAndItWasFirstTime()
        {
            var someBoard = new Board();
            someBoard.Movements = new List<Movement>();
            someBoard.NumberOfRows = 3;
            someBoard.NumberOfColumn = 3;
            var boardDealer = new BoardDealer();

            boardDealer.InitializeBoardConfiguration(someBoard);

            someBoard.FieldsConfiguration.Should().HaveCount(3);
            someBoard.FieldsConfiguration[0].Should().HaveCount(3);
            foreach (var somePlayer in someBoard.FieldsConfiguration[0])
                somePlayer.IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[1].Should().HaveCount(3);
            foreach (var somePlayer in someBoard.FieldsConfiguration[1])
                somePlayer.IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[2].Should().HaveCount(3);
            foreach (var somePlayer in someBoard.FieldsConfiguration[2])
                somePlayer.IsNull().Should().BeTrue();
            someBoard.FreeFields.Count.Should().Be(9);
            for (int position = 1; position <= someBoard.FreeFields.Count; position++)
                someBoard.FreeFields[position - 1].Should().Be(position);
        }

        /**
         * See each number below like a place in a tic tac toe board.
         * In this scenario the user chose the position 2.
         * 1 | 2x | 3
         * ---------
         * 4 | 5 | 6
         * ---------
         * 7 | 8 | 9
         */
        [Fact]
        public void ShouldInitializeBoardConfigurationGivenItsSetupAndSomePlayerAppliedMovementScenario1()
        {
            var someBoard = new Board();
            someBoard.Movements = new List<Movement>();
            var movement = new Movement();
            movement.Position = 2;
            var jafar = new Player();
            movement.WhoMade = jafar;
            someBoard.Movements.Add(movement);
            someBoard.NumberOfRows = 3;
            someBoard.NumberOfColumn = 3;
            var boardDealer = new BoardDealer();

            boardDealer.InitializeBoardConfiguration(someBoard);

            someBoard.FieldsConfiguration.Should().HaveCount(3);
            someBoard.FieldsConfiguration[0].Should().HaveCount(3);
            someBoard.FieldsConfiguration[0][0].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[0][1].Should().Be(jafar);
            someBoard.FieldsConfiguration[0][2].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[1].Should().HaveCount(3);
            foreach (var somePlayer in someBoard.FieldsConfiguration[1])
                somePlayer.IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[2].Should().HaveCount(3);
            foreach (var somePlayer in someBoard.FieldsConfiguration[2])
                somePlayer.IsNull().Should().BeTrue();
        }

        /**
         * See each number below like a place in a tic tac toe board.
         * In this scenario the user chose the position 2, 3, 6 and 7.
         * 1 | 2x | 3x
         * ---------
         * 4 | 5 | 6x
         * ---------
         * 7x | 8 | 9
         */
        [Fact]
        public void ShouldInitializeBoardConfigurationGivenItsSetupAndSomePlayerAppliedMovementScenario2()
        {
            var someBoard = new Board();
            var jafar = new Player();
            someBoard.Movements = new List<Movement>();
            var movement2 = new Movement();
            movement2.Position = 2;
            movement2.WhoMade = jafar;
            var movement3 = new Movement();
            movement3.Position = 3;
            movement3.WhoMade = jafar;
            var movement6 = new Movement();
            movement6.Position = 6;
            movement6.WhoMade = jafar;
            var movement7 = new Movement();
            movement7.Position = 7;
            movement7.WhoMade = jafar;
            someBoard.Movements.Add(movement2);
            someBoard.Movements.Add(movement3);
            someBoard.Movements.Add(movement6);
            someBoard.Movements.Add(movement7);
            someBoard.NumberOfRows = 3;
            someBoard.NumberOfColumn = 3;
            var boardDealer = new BoardDealer();

            boardDealer.InitializeBoardConfiguration(someBoard);

            someBoard.FieldsConfiguration.Should().HaveCount(3);
            someBoard.FieldsConfiguration[0].Should().HaveCount(3);
            someBoard.FieldsConfiguration[0][0].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[0][1].Should().Be(jafar);
            someBoard.FieldsConfiguration[0][2].Should().Be(jafar);
            someBoard.FieldsConfiguration[1].Should().HaveCount(3);
            someBoard.FieldsConfiguration[1][0].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[1][1].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[1][2].Should().Be(jafar);
            someBoard.FieldsConfiguration[2].Should().HaveCount(3);
            someBoard.FieldsConfiguration[2][0].Should().Be(jafar);
            someBoard.FieldsConfiguration[2][1].IsNull().Should().BeTrue();
            someBoard.FieldsConfiguration[2][2].IsNull().Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnRowAndColumnGivenSpecificPositionForBoard3x3()
        {
            var board = new Board();
            board.NumberOfColumn = 3;
            board.NumberOfRows = 3;
            var boardDealer = new BoardDealer();

            var (row, col) = boardDealer.GetRowAndColGivenAPosition(1, board);
            row.Should().Be(0);
            col.Should().Be(0);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(2, board);
            row.Should().Be(0);
            col.Should().Be(1);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(3, board);
            row.Should().Be(0);
            col.Should().Be(2);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(4, board);
            row.Should().Be(1);
            col.Should().Be(0);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(5, board);
            row.Should().Be(1);
            col.Should().Be(1);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(6, board);
            row.Should().Be(1);
            col.Should().Be(2);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(7, board);
            row.Should().Be(2);
            col.Should().Be(0);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(8, board);
            row.Should().Be(2);
            col.Should().Be(1);
            (row, col) = boardDealer.GetRowAndColGivenAPosition(9, board);
            row.Should().Be(2);
            col.Should().Be(1);            
        }
    }
}