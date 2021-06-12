using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class GamesControllerITests : ApiIntegrationTests
    {
        private readonly string _requestUri;

        public GamesControllerITests()
        {
            _requestUri = "api/v1/games";
        }

        [Fact]
        public async Task ShouldCreateGameGivenFirstMovementIsBeingExecuted()
        {
            // Arrange
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .Build()).First();
            var movementPosition = 1;
            var requestPath = $"{_requestUri}/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";
            // Act
            var response = await Client.GetAsync(_requestUri);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var gameStatus = await response.Content.ReadFromJsonAsync<Game>();
            gameStatus.Should().NotBe(null);
            gameStatus.Draw.Should().BeFalse();
            gameStatus.Finished.Should().BeFalse();
            gameStatus.Winner.Should().BeNull();
            var boardUsedToPlay = gameStatus.ConfiguredBoard;
            var boardPositions = boardUsedToPlay.NumberOfRows * boardUsedToPlay.NumberOfColumn;
            boardUsedToPlay.Movements.Count.Should().Be(2);
            var expectedFreeFields = boardPositions - boardUsedToPlay.Movements.Count;
            boardUsedToPlay.FreeFields.Count.Should().Be(expectedFreeFields);
            // All board positions
            boardUsedToPlay.FieldsConfiguration[0][0].Name.Should().Be(aladdin.Name);
            boardUsedToPlay.FieldsConfiguration[0][1].Name.Should().Be(rose.Name);
            boardUsedToPlay.FieldsConfiguration[0][2].Should().BeNull();
            for (var position = 1; position < boardUsedToPlay.FieldsConfiguration.Count; position++)
                foreach (var player in boardUsedToPlay.FieldsConfiguration[position])
                    player.Should().BeNull();
        }

        [Fact]
        public async Task ShouldRaise400GivenBoardIsNotFoundToCreateGame()
        {
            // Arrange
            var fakeBoardId = 42L;
            var fakePlayerId = 84L;
            var movementPosition = 1;
            var requestPath = $"{_requestUri}/{fakeBoardId}/{fakePlayerId}/{movementPosition}";
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Board not found");
        }

        [Fact]
        public async Task ShouldRaise400GivenPlayerIsNotFoundToPlayGame()
        {
            // Arrange
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(new Player {Name = "Aladdin"}, new Player {Name = "Rose"})
                .Build()).First();
            var fakePlayerId = 42L;
            var movementPosition = 1;
            var requestPath = $"{_requestUri}/{createdBoard.Id}/{fakePlayerId}/{movementPosition}";
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Player not found");
        }

        [Fact]
        public async Task ShouldExecuteThreeMovementsAndWinTheGame()
        {
            // Arrange
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .Build()).First();
            var movementsToWin = new List<int> {7, 8, 9};
            Game lastGameStatus = null;
            // Act and assert
            foreach (var movementPosition in movementsToWin)
            {
                var requestPath = $"{_requestUri}/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";
                var response = await Client.GetAsync(requestPath);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                if (movementPosition == 9)
                    lastGameStatus = await response.Content.ReadFromJsonAsync<Game>();
            }

            lastGameStatus.Draw.Should().BeFalse();
            lastGameStatus.Finished.Should().BeTrue();
            lastGameStatus.Winner.Name.Should().Be(aladdin.Name);
            lastGameStatus.Winner.Id.Should().Be(aladdin.Id);
            var boardUsedToPlay = lastGameStatus.ConfiguredBoard;
            var boardPositions = boardUsedToPlay.NumberOfRows * boardUsedToPlay.NumberOfColumn;
            boardUsedToPlay.Movements.Count.Should().Be(5);
            var expectedFreeFields = boardPositions - boardUsedToPlay.Movements.Count;
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

        [Fact]
        public async Task ShouldRaise400GivenTheGameIsFinished()
        {
            // Arrange
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .CreateGame(completed: true)
                .Build()).First();
            await new GameBuilder()
                .WithDbContext(AppDbContext)
                .WithBoard(createdBoard)
                .WithPlayers(aladdin, rose)
                .PlayerOneWinning()
                .Build();
            var movementPosition = 1;
            var requestPath = $"{_requestUri}/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Game not available to be played anymore");
        }
    }
}
