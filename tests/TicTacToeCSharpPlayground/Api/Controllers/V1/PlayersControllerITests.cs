using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DrfLikePaginations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Models;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Api.Controllers.V1
{
    public class PlayersControllerITests : IntegrationTestsWithDependencyInjection
    {
        private readonly string _requestUri;

        public PlayersControllerITests()
        {
            _requestUri = "api/v1/players";
        }

        [Fact]
        public async Task ShouldReturn404GivenNoPlayerIsFound()
        {
            // Arrange
            var fakePlayerId = 42;
            // Act
            var response = await Client.GetAsync($"{_requestUri}/{fakePlayerId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ShouldReturn400GivenPlayerIsInvalid()
        {
            // When nullable is activated, it can be used to assert if a given body is valid or not
            // Arrange
            var playerToBeCreated = new Player();
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, playerToBeCreated);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var valueAsString = await response.Content.ReadAsStringAsync();
            var responseJson = JObject.Parse(valueAsString);
            var errors = responseJson["errors"].ToObject<Dictionary<string, List<string>>>();
            errors.Keys.Should().HaveCount(1);
            var errorsRelatedToName = errors["Name"];
            errorsRelatedToName.Should().HaveCount(1);
            var motive = errorsRelatedToName.First();
            motive.Should().Be("The Name field is required.");
        }

        [Fact]
        public async Task ShouldCreatePlayerGivenValidRequest()
        {
            // Arrange
            var playerToBeCreated = new Player { Name = "Jafar" };
            // Act
            var response = await Client.PostAsJsonAsync(_requestUri, playerToBeCreated);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPlayer = await response.Content.ReadFromJsonAsync<Player>();
            var expectedLocation = $"http://localhost/{_requestUri}/{createdPlayer.Id}";
            response.Headers.Location.ToString().Should().Be(expectedLocation);
            createdPlayer.Name.Should().Be(playerToBeCreated.Name);
            createdPlayer.Computer.Should().BeFalse();
            createdPlayer.Id.Should().BePositive();
        }

        [Fact]
        public async Task ShouldReturnPlayerCreatedPreviously()
        {
            // Arrange
            var somePlayer = new Player { Name = "Salted Guy" };
            AppDbContext.Players.Add(somePlayer);
            await AppDbContext.SaveChangesAsync();
            // Act
            var response = await Client.GetAsync($"{_requestUri}/{somePlayer.Id}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedPlayer = await response.Content.ReadFromJsonAsync<Player>();
            retrievedPlayer.Should().BeEquivalentTo(somePlayer);
        }

        [Fact]
        public async Task ShouldDeletePlayer()
        {
            // Arrange
            var bear = new Player { Name = "Bear" };
            AppDbContext.Players.AddRange(bear, new Player { Name = "Salted Man" });
            await AppDbContext.SaveChangesAsync();
            AppDbContext.Players.Count().Should().Be(2);
            // Act
            var response = await Client.DeleteAsync($"{_requestUri}/{bear.Id}");
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var player = await response.Content.ReadFromJsonAsync<Player>();
            player.Id.Should().Be(bear.Id);
            AppDbContext.Players.Count().Should().Be(1);
        }

        [Fact]
        public async Task ShouldUpdatePlayer()
        {
            // Arrange
            var bear = new Player { Name = "Bear" };
            AppDbContext.Players.Add(bear);
            await AppDbContext.SaveChangesAsync();
            // This is needed because DbContext is Singleton (only during tests)
            AppDbContext.Entry(bear).State = EntityState.Detached;
            var bearWithNewName = new { bear.Id, Name = "Salted Bear", bear.Computer };
            var serializeObject = JsonConvert.SerializeObject(bearWithNewName);
            var contentRequest = new StringContent(serializeObject, Encoding.UTF8, "application/json");
            // Act
            var response = await Client.PutAsync(_requestUri, contentRequest);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            AppDbContext.Players.Count().Should().Be(1);
            var refreshedBear = AppDbContext.Players.First();
            refreshedBear.Name.Should().Be(bearWithNewName.Name);
        }

        [Fact]
        public async Task ShouldReturnAllPlayers()
        {
            // Arrange
            AppDbContext.Players.AddRange(new Player { Name = "Bear" }, new Player { Name = "Salted Man" });
            await AppDbContext.SaveChangesAsync();
            // Act
            var response = await Client.GetAsync(_requestUri);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paginatedPlayer = await response.Content.ReadFromJsonAsync<Paginated<PlayerDTO>>();
            paginatedPlayer.Count.Should().Be(2);
            paginatedPlayer.Results.Should().HaveCount(2);
        }
    }
}
