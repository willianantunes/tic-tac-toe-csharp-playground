using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using src.Repository;
using Xunit;

namespace tests.Integration.Controllers
{
    public class TicTacToeControllerTest : IClassFixture<WebApplicationFactory<src.Startup>>
    {
        private HttpClient _httpClient;
        private WebApplicationFactory<src.Startup> _factory;

        public TicTacToeControllerTest(WebApplicationFactory<src.Startup> factory)
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
        public async Task ShoudlReturnAllPlayers()
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
    }
}
