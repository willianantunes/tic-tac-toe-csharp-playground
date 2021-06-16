using System.Collections.Generic;
using FluentAssertions;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Models
{
    public class BoardTests
    {
        [Fact(DisplayName = "When initialization is called with a clean board (without movements)")]
        public void ShouldInitializeScenarioOne()
        {
            // Arrange
            var board = new Board { Movements = new List<Movement>(), NumberOfRows = 3, NumberOfColumn = 3 };
            // Act
            board.InitializeBoardConfiguration();
            // Assert
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
        [Fact(DisplayName = "When initialization is called given a board has one movement at position 2")]
        public void ShouldInitializeScenarioTwo()
        {
            // Arrange
            var player = new Player { Name = "Jafar" };
            var movement = new Movement { Position = 2, WhoMade = player };
            var board = new Board { Movements = new List<Movement> { movement }, NumberOfRows = 3, NumberOfColumn = 3 };
            // Act
            board.InitializeBoardConfiguration();
            // Assert
            board.FieldsConfiguration.Should().HaveCount(3);
            board.FieldsConfiguration[0].Should().HaveCount(3);
            board.FieldsConfiguration[0][0].Should().BeNull();
            board.FieldsConfiguration[0][1].Should().Be(player);
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
        [Fact(DisplayName = "When initialization is called given a board has movements at positions 2, 3, 6, and 7")]
        public void ShouldInitializeScenarioThree()
        {
            // Arrange
            var player = new Player { Name = "Iago" };
            var movement2 = new Movement { Position = 2, WhoMade = player };
            var movement3 = new Movement { Position = 3, WhoMade = player };
            var movement6 = new Movement { Position = 6, WhoMade = player };
            var movement7 = new Movement { Position = 7, WhoMade = player };
            var board = new Board
            {
                Movements = new List<Movement> { movement2, movement3, movement6, movement7 },
                NumberOfRows = 3,
                NumberOfColumn = 3
            };
            // Act
            board.InitializeBoardConfiguration();
            // Assert
            board.FieldsConfiguration.Should().HaveCount(3);
            board.FieldsConfiguration[0].Should().HaveCount(3);
            board.FieldsConfiguration[0][0].Should().BeNull();
            board.FieldsConfiguration[0][1].Should().Be(player);
            board.FieldsConfiguration[0][2].Should().Be(player);
            board.FieldsConfiguration[1].Should().HaveCount(3);
            board.FieldsConfiguration[1][0].Should().BeNull();
            board.FieldsConfiguration[1][1].Should().BeNull();
            board.FieldsConfiguration[1][2].Should().Be(player);
            board.FieldsConfiguration[2].Should().HaveCount(3);
            board.FieldsConfiguration[2][0].Should().Be(player);
            board.FieldsConfiguration[2][1].Should().BeNull();
            board.FieldsConfiguration[2][2].Should().BeNull();
        }
    }
}
