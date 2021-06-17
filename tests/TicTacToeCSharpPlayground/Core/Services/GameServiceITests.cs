using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Exceptions;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Services;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Core.Services
{
    public class GameServiceITests
    {
        public class CreatingBoard : IntegrationTestsWithDependencyInjection
        {
            private readonly IGameService _service;

            public CreatingBoard()
            {
                _service = Services.GetRequiredService<IGameService>();
            }

            [Fact(DisplayName = "Should throw exception when board size is not supported")]
            public async Task ShouldThrowExceptionScenarioOne()
            {
                // Arrange
                var wrongBoardSize = "4x3";
                var firstPlayerId = 1;
                var secondPlayerId = 2;
                // Act
                Func<Task> action = async () =>
                    await _service.CreateNewBoard(wrongBoardSize, firstPlayerId, secondPlayerId);
                // Assert
                await action.Should().ThrowAsync<InvalidBoardConfigurationException>()
                    .WithMessage($"Board {wrongBoardSize} is not supported. You can try 3x3 👍");
            }

            [Fact(DisplayName = "Should throw exception when either player one or two is not found")]
            public async Task ShouldThrowExceptionScenarioTwo()
            {
                // Arrange
                var wrongBoardSize = "3x3";
                var firstPlayerId = 1;
                var secondPlayerId = 2;
                // Act
                Func<Task> action = async () =>
                    await _service.CreateNewBoard(wrongBoardSize, firstPlayerId, secondPlayerId);
                // Assert
                await action.Should().ThrowAsync<PlayerNotFoundException>()
                    .WithMessage($"Both players are required. P1: ❓ | P2: ❓");
            }

            [Fact]
            public async Task ShouldCreateBoard()
            {
                // Arrange
                var playerOne = new Player { Name = "Bear" };
                var playerTwo = new Player { Name = "Salted Man" };
                AppDbContext.Players.AddRange(playerOne, playerTwo);
                await AppDbContext.SaveChangesAsync();
                var wrongBoardSize = "3x3";
                // Act
                var board = await _service.CreateNewBoard(wrongBoardSize, playerOne.Id, playerTwo.Id);
                // Assert
                AppDbContext.Boards.Should().HaveCount(1);
                AppDbContext.PlayerBoards.Should().HaveCount(2);
                board.Id.Should().Be(1);
            }
        }

        public class DealingWithGame : IntegrationTestsWithDependencyInjection
        {
            private readonly IGameService _service;

            // In order to add the dummy decider
            private static Action<IServiceCollection> ProvideCustomSetup()
            {
                return services =>
                {
                    services.RemoveAll<IPositionDecider>();
                    services.AddSingleton<IPositionDecider, CustomPositionDecider>();
                };
            }

            public DealingWithGame() : base(ProvideCustomSetup())
            {
                _service = Services.GetRequiredService<IGameService>();
            }

            [Fact(DisplayName = "Should throw exception when board is not found")]
            public async Task ShouldThrowExceptionScenarioOne()
            {
                // Arrange
                var boardId = 42;
                var playerId = 6;
                var position = 9;
                // Act
                Func<Task> action = async () =>
                    await _service.ExecuteMovementAndRetrieveGameStatus(boardId, playerId, position);
                // Assert
                var expectedMessage = $"The board {boardId} is not available. Are you sure you are correct? 🤔";
                await action.Should().ThrowAsync<BoardNotFoundToBePlayedException>()
                    .WithMessage(expectedMessage);
            }

            [Fact(DisplayName = "Should throw exception when player not found")]
            public async Task ShouldThrowExceptionScenarioTwo()
            {
                // Arrange
                var createdBoard = await new BoardBuilder()
                    .WithDbContext(AppDbContext)
                    .CreateBoard()
                    .WithPlayers(new Player { Name = "Aladdin" }, new Player { Name = "Rose" })
                    .BuildAndGetFirstBoard();
                var playerId = 42;
                var position = 1;
                // Act
                Func<Task> action = async () =>
                    await _service.ExecuteMovementAndRetrieveGameStatus(createdBoard.Id, playerId, position);
                // Assert
                var expectedMessage = $"There is no player with ID {playerId}";
                await action.Should().ThrowAsync<PlayerNotFoundException>()
                    .WithMessage(expectedMessage);
            }

            [Fact(DisplayName = "Should throw exception when provided player is a robot")]
            public async Task ShouldThrowExceptionScenarioThree()
            {
                // Arrange
                var robot = new Player { Name = "Rose", Computer = true };
                var createdBoard = await new BoardBuilder()
                    .WithDbContext(AppDbContext)
                    .CreateBoard()
                    .WithPlayers(new Player { Name = "Aladdin" }, robot)
                    .BuildAndGetFirstBoard();
                var position = 1;
                // Act
                Func<Task> action = async () =>
                    await _service.ExecuteMovementAndRetrieveGameStatus(createdBoard.Id, robot.Id, position);
                // Assert
                var expectedMessage = $"{robot.Name} is a robot. Only I can use it!";
                await action.Should().ThrowAsync<YouAreNotAllowedToPlayWithARobotException>()
                    .WithMessage(expectedMessage);
            }

            [Fact(DisplayName = "Should throw exception when game is finished")]
            public async Task ShouldThrowExceptionScenarioFour()
            {
                // Arrange
                var aladdin = new Player { Name = "Aladdin", Computer = false };
                var rose = new Player { Name = "Rose", Computer = true };
                var createdBoard = await new BoardBuilder()
                    .WithDbContext(AppDbContext)
                    .CreateBoard()
                    .WithPlayers(aladdin, rose)
                    .BuildAndGetFirstBoard();
                var game = new Game
                {
                    Draw = false,
                    Finished = true,
                    Winner = aladdin,
                    ConfiguredBoard = createdBoard
                };
                AppDbContext.Games.Add(game);
                await AppDbContext.SaveChangesAsync();
                var position = 1;
                // Act
                Func<Task> action = async () =>
                    await _service.ExecuteMovementAndRetrieveGameStatus(createdBoard.Id, aladdin.Id, position);
                // Assert
                var expectedMessage = $"The game associated with the board {createdBoard.Id} is finished";
                await action.Should().ThrowAsync<GameIsNotPlayableException>()
                    .WithMessage(expectedMessage);
            }

            [Fact(DisplayName = "Should throw exception when position is not available")]
            public async Task ShouldThrowExceptionScenarioFive()
            {
                // Arrange
                var aladdin = new Player { Name = "Aladdin", Computer = false };
                var rose = new Player { Name = "Rose", Computer = true };
                var board = await new BoardBuilder()
                    .WithDbContext(AppDbContext)
                    .CreateBoard()
                    .WithPlayers(aladdin, rose)
                    .BuildAndGetFirstBoard();
                var position = 42;
                // Act
                Func<Task> action = async () =>
                    await _service.ExecuteMovementAndRetrieveGameStatus(board.Id, aladdin.Id, position);
                // Assert
                var positions = String.Join(" ", board.FreeFields);
                var expectedMessage = $"Position {position} is not available. The ones you can choose: {positions}";
                await action.Should().ThrowAsync<PositionNotAvailableException>()
                    .WithMessage(expectedMessage);
            }

            [Fact(DisplayName = "Should execute three movements and finish the game")]
            public async Task ShouldExecuteMovementScenarioOne()
            {
                // Arrange
                var aladdin = new Player { Name = "Aladdin", Computer = false };
                var rose = new Player { Name = "Rose", Computer = true };
                var board = await new BoardBuilder()
                    .WithDbContext(AppDbContext)
                    .CreateBoard()
                    .WithPlayers(aladdin, rose)
                    .BuildAndGetFirstBoard();
                var movementsToWin = new[] { 7, 8, 9 };
                // Act
                GameDTO lastGameStatus = null;
                foreach (var movementPosition in movementsToWin)
                {
                    lastGameStatus =
                        await _service.ExecuteMovementAndRetrieveGameStatus(board.Id, aladdin.Id, movementPosition);
                }

                // Assert
                AppDbContext.Boards.Should().HaveCount(1);
                AppDbContext.Games.Should().HaveCount(1);
                AppDbContext.Players.Should().HaveCount(2);
                AppDbContext.PlayerBoards.Should().HaveCount(2);
                AppDbContext.Movements.Should().HaveCount(5);
                lastGameStatus.Draw.Should().BeFalse();
                lastGameStatus.Finished.Should().BeTrue();
                lastGameStatus.Winner.Name.Should().Be(aladdin.Name);
                lastGameStatus.Winner.Id.Should().Be(aladdin.Id);
                var boardUsedToPlay = lastGameStatus.ConfiguredBoard;
                var boardPositions = boardUsedToPlay.NumberOfRows * boardUsedToPlay.NumberOfColumn;
                var expectedFreeFields = boardPositions - AppDbContext.Movements.Count();
                boardUsedToPlay.FreeFields.Count.Should().Be(expectedFreeFields);
                // All board positions
                boardUsedToPlay.FieldsConfiguration[2][0].Name.Should().Be(aladdin.Name);
                boardUsedToPlay.FieldsConfiguration[2][1].Name.Should().Be(aladdin.Name);
                boardUsedToPlay.FieldsConfiguration[2][2].Name.Should().Be(aladdin.Name);
                var availablePositions = 0;
                for (var position = 0; position < 2; position++)
                    foreach (var player in boardUsedToPlay.FieldsConfiguration[position])
                    {
                        if (player is null)
                            availablePositions++;
                    }

                availablePositions.Should().Be(4);
            }
        }
    }
}
