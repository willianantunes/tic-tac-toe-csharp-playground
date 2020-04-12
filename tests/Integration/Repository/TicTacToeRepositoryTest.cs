using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using src.Repository;
using Xunit;

namespace tests.Integration.Repository
{
    public class TicTacToeRepository : IClassFixture<WebApplicationFactory<src.Startup>>
    {
        private WebApplicationFactory<src.Startup> _factory;
        private HttpClient _httpClient;

        public TicTacToeRepository(WebApplicationFactory<src.Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ShouldReturnNullGivenNoBoardWasFound()
        {
            using var scope = _factory.Services.CreateScope();
            var ticTacToeRepository = scope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();
            var guid = Guid.NewGuid();
            var board = await ticTacToeRepository.GetBoardByItsId(guid);

            board.Should().BeNull();
        }
    }
}
