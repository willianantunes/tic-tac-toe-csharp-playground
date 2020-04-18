using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using src;
using src.Helper;
using src.Repository;
using tests.Resources;
using Xunit;

namespace tests.Integration.Controllers
{
    public class TicTacToeControllerTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<Startup> _factory;

        public TicTacToeControllerTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task ShouldCreatePlayerGivenValidRequest()
        {
            var playerToBeCreated = new Player {Name = "Jafar"};

            var response = await _httpClient.PostAsJsonAsync("/tic-tac-toe/players", playerToBeCreated);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPlayer = await response.Content.ReadAsAsync<Player>();
            response.Headers.Location.ToString().Should()
                .Be($"http://localhost/tic-tac-toe/players/{createdPlayer.Id}");

            createdPlayer.Name.Should().Be(playerToBeCreated.Name);
            createdPlayer.Computer.Should().BeFalse();
            createdPlayer.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ShouldReturn400GivenPlayerIsInvalid()
        {
            var playerToBeCreated = new Player();

            var response = await _httpClient.PostAsJsonAsync("/tic-tac-toe/players", playerToBeCreated);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var motive = await response.Content.ReadAsStringAsync();

            motive.Should().Be("Name is required to create a player");
        }

        [Fact]
        public async Task ShouldReturnPlayerCreatedPreviously()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            var somePlayer = new Player {Name = "Chumaço"};
            context.Players.Add(somePlayer);
            await context.SaveChangesAsync();

            var response = await _httpClient.GetAsync($"/tic-tac-toe/players/{somePlayer.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedPlayer = await response.Content.ReadAsAsync<Player>();

            retrievedPlayer.Should().BeEquivalentTo(somePlayer);
        }

        [Fact]
        public async Task ShouldReturn404GivenNoPlayerIsFound()
        {
            var fakePlayerGuid = "fd445dcb-dc37-4679-b474-f7fb0512f607";

            // TODO: Verify if FindAsync is indeed called 
            var response = await _httpClient.GetAsync($"/tic-tac-toe/players/{fakePlayerGuid}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ShouldReturnAllPlayers()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.Players.RemoveRange(context.Players);
            await context.SaveChangesAsync();
            // ADD DATA
            context.Players.AddRange(new Player {Name = "Chumaço"}, new Player {Name = "Salted Man"});
            await context.SaveChangesAsync();

            var response = await _httpClient.GetAsync("/tic-tac-toe/players");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var players = await response.Content.ReadAsAsync<List<Player>>();

            players.Count.Should().Be(2);
        }

        [Fact]
        public async Task ShouldCreateBoardGivenAtLeastOnePlayerWasProvidedAndWithProvidedBoardSetup()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.Games.RemoveRange(context.Games);
            context.Movements.RemoveRange(context.Movements);
            context.Boards.RemoveRange(context.Boards);
            context.Players.RemoveRange(context.Players);
            await context.SaveChangesAsync();
            // ADD DATA
            var antunes = new Player {Name = "Antunes"};
            var rose = new Player {Name = "Rose", Computer = true};
            context.Players.AddRange(antunes, rose);
            await context.SaveChangesAsync();

            var postData = new {boardSize = "4x4", firstPlayerId = antunes.Id.ToString()};

            var response = await _httpClient.PostAsJsonAsync("/tic-tac-toe/boards", postData);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdBoard = await response.Content.ReadAsAsync<Board>();

            createdBoard.Should().NotBe(null);
            createdBoard.NumberOfColumn.Should().Be(4);
            createdBoard.NumberOfRows.Should().Be(4);
            createdBoard.PlayerBoards.Count.Should().Be(2);
            createdBoard.Movements.Should().BeNull();
        }

        [Fact]
        public async Task ShouldCreateDefaultBoardWithStandardSetupGivenTwoPlayersProvided()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.Games.RemoveRange(context.Games);
            context.Movements.RemoveRange(context.Movements);
            context.Boards.RemoveRange(context.Boards);
            context.Players.RemoveRange(context.Players);
            await context.SaveChangesAsync();
            // ADD DATA
            var aladdin = new Player {Name = "Aladdin"};
            var jasmine = new Player {Name = "Jasmine"};
            await context.Players.AddRangeAsync(aladdin, jasmine);
            await context.SaveChangesAsync();

            var postData = new
            {
                firstPlayerId = aladdin.Id.ToString(),
                secondPlayerId = jasmine.Id.ToString()
            };
            var response = await _httpClient.PostAsJsonAsync("/tic-tac-toe/boards", postData);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdBoard = await response.Content.ReadAsAsync<Board>();

            createdBoard.Should().NotBe(null);
            createdBoard.NumberOfColumn.Should().Be(3);
            createdBoard.NumberOfRows.Should().Be(3);
            var playerBoards = createdBoard.PlayerBoards;
            playerBoards.Count.Should().Be(2);
            playerBoards.FirstOrDefault(pb => pb.Player.Id == aladdin.Id).Should().NotBeNull();
            playerBoards.FirstOrDefault(pb => pb.Player.Id == jasmine.Id).Should().NotBeNull();
            createdBoard.Movements.Should().BeNull();
        }

        [Fact]
        public async Task ShouldRaise400GivenBoardSetupIsNotValid()
        {
            var postData = new {boardSize = "2x2"};

            var response = await _httpClient.PostAsJsonAsync("/tic-tac-toe/boards", postData);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Board configuration not valid");
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
