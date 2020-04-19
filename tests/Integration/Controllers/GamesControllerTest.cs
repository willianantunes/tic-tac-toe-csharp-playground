using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeCSharpPlayground;
using TicTacToeCSharpPlayground.Helper;
using TicTacToeCSharpPlayground.Repository;
using tests.Resources;
using Xunit;

namespace tests.Integration.Controllers
{
    public class GamesControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<Startup> _factory;

        public GamesControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task ShouldCreateGameGivenFirstMovementIsBeingExecuted()
        {
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};

            var createdBoard = (await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .Build()).First();

            var movementPosition = 1;
            var requestPath = $"/tic-tac-toe/games/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";

            var response = await _httpClient.GetAsync(requestPath);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var gameStatus = await response.Content.ReadAsAsync<Game>();

            gameStatus.Should().NotBe(null);
            gameStatus.Draw.Should().BeFalse();
            gameStatus.Finished.Should().BeFalse();
            gameStatus.Winner.Should().BeNull();
            var boardUsedToPlay = gameStatus.ConfiguredBoard;
            var boardPositions = boardUsedToPlay.NumberOfRows * boardUsedToPlay.NumberOfColumn;
            boardUsedToPlay.Movements.Count.Should().Be(2);
            var expectedFreeFields = boardPositions - boardUsedToPlay.Movements.Count;
            boardUsedToPlay.FreeFields.Count.Should().Be(expectedFreeFields);
            // ASSERTING ALL BOARD POSITIONS
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
            Guid fakeBoardId = new Guid();
            Guid fakePlayerId = new Guid();
            var movementPosition = 1;

            var requestPath = $"/tic-tac-toe/games/{fakeBoardId}/{fakePlayerId}/{movementPosition}";
            var response = await _httpClient.GetAsync(requestPath);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Board not found");
        }

        [Fact]
        public async Task ShouldRaise400GivenPlayerIsNotFoundToPlayGame()
        {
            var createdBoard = (await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .WithPlayers(new Player {Name = "Aladdin"}, new Player {Name = "Rose"})
                .Build()).First();

            Guid fakePlayerId = new Guid();
            var movementPosition = 1;

            var requestPath = $"/tic-tac-toe/games/{createdBoard.Id}/{fakePlayerId}/{movementPosition}";

            var response = await _httpClient.GetAsync(requestPath);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Player not found");
        }

        [Fact]
        public async Task ShouldExecuteThreeMovementsAndWinTheGame()
        {
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};

            var createdBoard = (await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .Build()).First();

            var movementsToWin = new List<int> {7, 8, 9};
            Game lastGameStatus = null;
            foreach (var movementPosition in movementsToWin)
            {
                var requestPath = $"/tic-tac-toe/games/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";
                var response = await _httpClient.GetAsync(requestPath);
                response.StatusCode.Should().Be(HttpStatusCode.OK);
                if (movementPosition == 9)
                    lastGameStatus = await response.Content.ReadAsAsync<Game>();
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
            // ASSERTING ALL BOARD POSITIONS
            boardUsedToPlay.FieldsConfiguration[2][0].Name.Should().Be(aladdin.Name);
            boardUsedToPlay.FieldsConfiguration[2][1].Name.Should().Be(aladdin.Name);
            boardUsedToPlay.FieldsConfiguration[2][2].Name.Should().Be(aladdin.Name);
            var availablePositions = 0;
            for (var position = 0; position < 2; position++)
                foreach (var player in boardUsedToPlay.FieldsConfiguration[position])
                {
                    if (player.IsNull())
                        availablePositions++;
                }

            availablePositions.Should().Be(4);
        }

        [Fact]
        public async Task ShouldRaise400GivenTheGameIsFinished()
        {
            var aladdin = new Player {Name = "Aladdin", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};

            var createdBoard = (await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .CreateGame(completed: true)
                .Build()).First();

            await new GameBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .WithBoard(createdBoard)
                .WithPlayers(aladdin, rose)
                .PlayerOneWinning()
                .Build();

            var movementPosition = 1;
            var requestPath = $"/tic-tac-toe/games/{createdBoard.Id}/{aladdin.Id}/{movementPosition}";

            var response = await _httpClient.GetAsync(requestPath);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Game not available to be played anymore");
        }
    }
}
