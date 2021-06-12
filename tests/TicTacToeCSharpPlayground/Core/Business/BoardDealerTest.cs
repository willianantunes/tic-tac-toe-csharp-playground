using System.Collections.Generic;
using FluentAssertions;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Business
{
    public class BoardDealerTest
    {
        private readonly IBoardDealer _boardDealer = new BoardDealer();

        [Fact]
        public void ShouldReturnFalseIfBoardSizeIsNull()
        {
            _boardDealer.NotValidOrUnsupportedBoardSize(null).Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfBoardSizeDoesNotMatchRegex()
        {
            _boardDealer.NotValidOrUnsupportedBoardSize("Calopsita").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("3").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("1v1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2v2").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("3v3").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("4x3").Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnTrueIfBoardSizeHasColumnAndRowsGreaterThan2AndAreEqualAndLessThan10()
        {
            // TODO: A loop can be used here
            _boardDealer.NotValidOrUnsupportedBoardSize("1x1").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("2x2").Should().BeTrue();
            _boardDealer.NotValidOrUnsupportedBoardSize("3x3").Should().BeFalse();
            _boardDealer.NotValidOrUnsupportedBoardSize("4x4").Should().BeFalse();
            _boardDealer.NotValidOrUnsupportedBoardSize("9x9").Should().BeFalse();
            _boardDealer.NotValidOrUnsupportedBoardSize("10x10").Should().BeTrue();
        }

        [Fact]
        public void ShouldInitializeBoardConfigurationGivenItsSetupAndItWasFirstTime()
        {
            var board = new Board();
            board.Movements = new List<Movement>();
            board.NumberOfRows = 3;
            board.NumberOfColumn = 3;

            _boardDealer.InitializeBoardConfiguration(board);

            board.FieldsConfiguration.Should().HaveCount(3);
            board.FieldsConfiguration[0].Should().HaveCount(3);
            foreach (var somePlayer in board.FieldsConfiguration[0])
                somePlayer.Should().BeNull();
            board.FieldsConfiguration[1].Should().HaveCount(3);
            foreach (var somePlayer in board.FieldsConfiguration[1])
                somePlayer.Should().BeNull();
            board.FieldsConfiguration[2].Should().HaveCount(3);
            foreach (var somePlayer in board.FieldsConfiguration[2])
                somePlayer.Should().BeNull();
            board.FreeFields.Count.Should().Be(9);
            for (int position = 1; position <= board.FreeFields.Count; position++)
                board.FreeFields[position - 1].Should().Be(position);
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
            var board = new Board();
            board.Movements = new List<Movement>();
            var movement = new Movement();
            movement.Position = 2;
            var jafar = new Player();
            movement.WhoMade = jafar;
            board.Movements.Add(movement);
            board.NumberOfRows = 3;
            board.NumberOfColumn = 3;

            _boardDealer.InitializeBoardConfiguration(board);

            board.FieldsConfiguration.Should().HaveCount(3);
            board.FieldsConfiguration[0].Should().HaveCount(3);
            board.FieldsConfiguration[0][0].Should().BeNull();
            board.FieldsConfiguration[0][1].Should().Be(jafar);
            board.FieldsConfiguration[0][2].Should().BeNull();
            board.FieldsConfiguration[1].Should().HaveCount(3);
            foreach (var somePlayer in board.FieldsConfiguration[1])
                somePlayer.Should().BeNull();
            board.FieldsConfiguration[2].Should().HaveCount(3);
            foreach (var somePlayer in board.FieldsConfiguration[2])
                somePlayer.Should().BeNull();
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
            var board = new Board();
            var jafar = new Player();
            board.Movements = new List<Movement>();
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
            board.Movements.Add(movement2);
            board.Movements.Add(movement3);
            board.Movements.Add(movement6);
            board.Movements.Add(movement7);
            board.NumberOfRows = 3;
            board.NumberOfColumn = 3;

            _boardDealer.InitializeBoardConfiguration(board);

            board.FieldsConfiguration.Should().HaveCount(3);
            board.FieldsConfiguration[0].Should().HaveCount(3);
            board.FieldsConfiguration[0][0].Should().BeNull();
            board.FieldsConfiguration[0][1].Should().Be(jafar);
            board.FieldsConfiguration[0][2].Should().Be(jafar);
            board.FieldsConfiguration[1].Should().HaveCount(3);
            board.FieldsConfiguration[1][0].Should().BeNull();
            board.FieldsConfiguration[1][1].Should().BeNull();
            board.FieldsConfiguration[1][2].Should().Be(jafar);
            board.FieldsConfiguration[2].Should().HaveCount(3);
            board.FieldsConfiguration[2][0].Should().Be(jafar);
            board.FieldsConfiguration[2][1].Should().BeNull();
            board.FieldsConfiguration[2][2].Should().BeNull();
        }
    }
}
