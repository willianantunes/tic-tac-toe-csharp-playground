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

        [Fact]
        public async Task ShouldReturnBoardGivenItsId()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            var createdBoard = new Board();
            context.Boards.Add(createdBoard);
            await context.SaveChangesAsync();
            
            using var testScope = _factory.Services.CreateScope();
            var ticTacToeRepository = testScope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();
            var foundBoard = await ticTacToeRepository.GetBoardByItsId(createdBoard.Id);

            foundBoard.Should().Equals(createdBoard);
        }
    }
}
