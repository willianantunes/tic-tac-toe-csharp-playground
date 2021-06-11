using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TicTacToeCSharpPlayground;
using TicTacToeCSharpPlayground.EntryCommands;
using TicTacToeCSharpPlayground.Repository;
using Xunit;

namespace tests.Integration.Controllers
{
    public class BoardsControllerTest : IClassFixture<WebApplicationFactory<ApiCommand.Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<ApiCommand.Startup> _factory;

        public BoardsControllerTest(WebApplicationFactory<ApiCommand.Startup> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient();
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
            var createdBoard = await response.Content.ReadFromJsonAsync<Board>();

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
            var createdBoard = await response.Content.ReadFromJsonAsync<Board>();

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
    }
}
