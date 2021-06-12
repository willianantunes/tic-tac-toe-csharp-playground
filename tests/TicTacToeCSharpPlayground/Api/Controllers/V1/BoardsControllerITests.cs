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
    public class BoardsControllerITests : ApiIntegrationTests
    {
        private readonly string _requestUri;

        public BoardsControllerITests()
        {
            _requestUri = "api/v1/boards";
        }

        [Fact]
        public async Task ShouldCreateBoardGivenAtLeastOnePlayerWasProvidedAndWithProvidedBoardSetup()
        {
            // Arrange
            var antunes = new Player {Name = "Antunes"};
            var rose = new Player {Name = "Rose", Computer = true};
            AppDbContext.Players.AddRange(antunes, rose);
            await AppDbContext.SaveChangesAsync();
            var postData = new {boardSize = "4x4", firstPlayerId = antunes.Id.ToString()};
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, postData);
            // Assert
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
            // Arrange
            var aladdin = new Player {Name = "Aladdin"};
            var jasmine = new Player {Name = "Jasmine"};
            await AppDbContext.Players.AddRangeAsync(aladdin, jasmine);
            await AppDbContext.SaveChangesAsync();
            var postData = new
            {
                firstPlayerId = aladdin.Id.ToString(),
                secondPlayerId = jasmine.Id.ToString()
            };
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, postData);
            // Assert
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
            // Arrange
            var postData = new {boardSize = "2x2"};
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, postData);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("Board configuration not valid");
        }
    }
}
