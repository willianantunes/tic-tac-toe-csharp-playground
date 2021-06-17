using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class BoardsControllerITests : IntegrationTestsWithDependencyInjection
    {
        private readonly string _requestUri;

        public BoardsControllerITests()
        {
            _requestUri = "api/v1/boards";
        }

        [Fact]
        public async Task ShouldCreateBoardGivenProvidedBoardSetup()
        {
            // Arrange
            var antunes = new Player { Name = "Antunes" };
            var rose = new Player { Name = "Rose", Computer = true };
            AppDbContext.Players.AddRange(antunes, rose);
            await AppDbContext.SaveChangesAsync();
            var postData = new
            {
                boardSize = "4x4",
                firstPlayerId = antunes.Id,
                SecondPlayerId = rose.Id
            };
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, postData);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdBoard = await response.Content.ReadFromJsonAsync<BoardDTO>();
            createdBoard.Should().NotBe(null);
            createdBoard.NumberOfColumn.Should().Be(4);
            createdBoard.NumberOfRows.Should().Be(4);
            createdBoard.Players.Count.Should().Be(2);
        }

        [Fact]
        public async Task ShouldCreateDefaultBoardWithStandardSetupGivenTwoPlayersProvided()
        {
            // Arrange
            var aladdin = new Player { Name = "Aladdin" };
            var jasmine = new Player { Name = "Jasmine" };
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
            var createdBoard = await response.Content.ReadFromJsonAsync<BoardDTO>();
            createdBoard.Should().NotBe(null);
            createdBoard.NumberOfColumn.Should().Be(3);
            createdBoard.NumberOfRows.Should().Be(3);
            var playerBoards = createdBoard.Players;
            playerBoards.Count.Should().Be(2);
            playerBoards.FirstOrDefault(p => p.Id == aladdin.Id).Should().NotBeNull();
            playerBoards.FirstOrDefault(p => p.Id == jasmine.Id).Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldRaise400GivenBoardSetupIsNotValid()
        {
            // Arrange
            var postData = new { boardSize = "2x2" };
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, postData);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            var expectedMessage = $"Board {postData.boardSize} is not supported. You can try 3x3 👍";
            content.Should().Be(expectedMessage);
        }
    }
}
