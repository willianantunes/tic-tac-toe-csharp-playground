using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using src.Repository;
using tests.Resources;
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
            IList<Board> boards = await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .Build();

            var createdBoard = boards.First();
            using var testScope = _factory.Services.CreateScope();
            var ticTacToeRepository = testScope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();
            
            var foundBoard = await ticTacToeRepository.GetBoardByItsId(createdBoard.Id);

            foundBoard.Should().Equals(createdBoard);
        }
    }
}
