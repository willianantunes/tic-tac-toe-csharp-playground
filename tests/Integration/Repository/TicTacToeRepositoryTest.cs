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

        [Fact]
        public async Task ShouldReturnSomeComputerUser()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.Players.RemoveRange(context.Players);
            await context.SaveChangesAsync();
            // SEED DATA
            context.AddRange(new Player {Name = "Rose", Computer = true}, new Player {Name = "Z", Computer = true});
            await context.SaveChangesAsync();
            
            // THE TEST
            using var testScope = _factory.Services.CreateScope();
            var ticTacToeRepository = testScope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();

            var foundBoard = await ticTacToeRepository.GetSomeComputerPlayer();

            foundBoard.Computer.Should().BeTrue();
            foundBoard.Name.Should().ContainAny(new List<string> {"Rose", "Z"});
        }

        [Fact]
        public async Task ShouldSaveBoardWithItsPlayerBoards()
        {
            using var testPreparationScope = _factory.Services.CreateScope();
            var context = testPreparationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            // CLEAR OLD DATA
            context.Boards.RemoveRange(context.Boards);
            context.Players.RemoveRange(context.Players);
            await context.SaveChangesAsync();
            // SEED DATA
            var jafar = new Player {Name = "Jafar", Computer = false};
            var rose = new Player {Name = "Rose", Computer = true};
            context.Players.AddRange(jafar, rose);
            await context.SaveChangesAsync();
            
            // THE TEST
            using var testScope = _factory.Services.CreateScope();
            var ticTacToeRepository = testScope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();
            var boardToBeCreated = new Board{NumberOfColumn = 3, NumberOfRows = 3};
            var playerBoardOne = new PlayerBoard{Player = jafar, Board = boardToBeCreated };
            // var playerBoarTwo = new PlayerBoard{Player = rose, Board = boardToBeCreated};
            boardToBeCreated.PlayerBoards = new List<PlayerBoard>{playerBoardOne};
            await ticTacToeRepository.SaveBoard(boardToBeCreated);

            // Validation
            using var testValidationScope = _factory.Services.CreateScope();
            var contextValidation = testValidationScope.ServiceProvider.GetRequiredService<CSharpPlaygroundContext>();
            contextValidation.Boards.Count().Should().Be(1);
            contextValidation.Players.Count().Should().Be(2);
        }

        [Fact]
        public async Task ShouldCreateMovementAndRefreshBoardState()
        {
            var aladdin = new Player{Name = "Aladdin", Computer = false};
            var createdBoard = (await new BoardBuilder()
                .WithCreatedScopeFromServiceProvider(_factory.Services)
                .CreateBoard()
                .WithPlayers(aladdin)
                .Build()).First();
            
            
            using var testScope = _factory.Services.CreateScope();
            var ticTacToeRepository = testScope.ServiceProvider.GetRequiredService<ITicTacToeRepository>();
            
            // THE TEST
            var movement = new Movement {Position = 1, WhoMade = aladdin};
            await ticTacToeRepository.CreateMovementAndRefreshBoard(movement, createdBoard);

            createdBoard.Movements.Count.Should().Be(1);
        }
    }
}
