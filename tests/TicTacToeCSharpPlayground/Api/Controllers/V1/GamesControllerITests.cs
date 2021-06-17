using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class GamesControllerITests : IntegrationTestsWithDependencyInjection
    {
        private readonly string _baseRequestPath;
        private readonly string _requestPathPlay;

        // In order to add the dummy decider
        private static Action<IServiceCollection> ProvideCustomSetup()
        {
            return services =>
            {
                services.RemoveAll<IPositionDecider>();
                services.AddSingleton<IPositionDecider, CustomPositionDecider>();
            };
        }

        public GamesControllerITests() : base(ProvideCustomSetup())
        {
            _baseRequestPath = "api/v1/games";
            _requestPathPlay = "api/v1/games/play";
        }

        [Fact]
        public async Task ShouldCreateGameGivenFirstMovementIsBeingExecuted()
        {
            // Arrange
            var aladdin = new Player { Name = "Aladdin", Computer = false };
            var rose = new Player { Name = "Rose", Computer = true };
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .Build()).First();
            var movementPosition = 1;
            string requestPath = new BuildUri(_requestPathPlay)
                .AddParam("BoardId", createdBoard.Id)
                .AddParam("PlayerId", aladdin.Id)
                .AddParam("MovementPosition", movementPosition)
                .Build();
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var test = await response.Content.ReadAsStringAsync();
            var gameStatus = await response.Content.ReadFromJsonAsync<GameDTO>();
            gameStatus.Should().NotBe(null);
            gameStatus.Draw.Should().BeFalse();
            gameStatus.Finished.Should().BeFalse();
            gameStatus.Winner.Should().BeNull();
            var boardUsedToPlay = gameStatus.ConfiguredBoard;
            var boardPositions = boardUsedToPlay.NumberOfRows * boardUsedToPlay.NumberOfColumn;
            var expectedFreeFields = boardPositions - AppDbContext.Movements.Count();
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
            string requestPath = new BuildUri(_requestPathPlay)
                .AddParam("BoardId", fakeBoardId)
                .AddParam("PlayerId", fakePlayerId)
                .AddParam("MovementPosition", movementPosition)
                .Build();
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be($"The board {fakeBoardId} is not available. Are you sure you are correct? 🤔");
        }

        [Fact]
        public async Task ShouldRaise400GivenPlayerIsNotFoundToPlayGame()
        {
            // Arrange
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(new Player { Name = "Aladdin" }, new Player { Name = "Rose" })
                .Build()).First();
            var fakePlayerId = 42L;
            var movementPosition = 1;
            string requestPath = new BuildUri(_requestPathPlay)
                .AddParam("BoardId", createdBoard.Id)
                .AddParam("PlayerId", fakePlayerId)
                .AddParam("MovementPosition", movementPosition)
                .Build();
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be($"There is no player with ID {fakePlayerId}");
        }

        [Fact]
        public async Task ShouldRaise400GivenTheGameIsFinished()
        {
            // Arrange
            var aladdin = new Player { Name = "Aladdin", Computer = false };
            var rose = new Player { Name = "Rose", Computer = true };
            var board = await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin, rose)
                .BuildAndGetFirstBoard();
            await new GameBuilder()
                .WithDbContext(AppDbContext)
                .WithBoard(board)
                .WithPlayers(aladdin, rose)
                .PlayerOneWinning()
                .Build();
            var movementPosition = 1;
            string requestPath = new BuildUri(_requestPathPlay)
                .AddParam("BoardId", board.Id)
                .AddParam("PlayerId", aladdin.Id)
                .AddParam("MovementPosition", movementPosition)
                .Build();
            // Act
            var response = await Client.GetAsync(requestPath);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be($"The game associated with the board {board.Id} is finished");
        }
    }
}
