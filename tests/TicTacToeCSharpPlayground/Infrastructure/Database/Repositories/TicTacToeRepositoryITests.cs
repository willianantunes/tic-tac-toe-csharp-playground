using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Tests.Support;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Repository;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.Infrastructure.Database.Repositories
{
    public class TicTacToeRepositoryITests : IntegrationTestsWithDependencyInjection
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;

        public TicTacToeRepositoryITests()
        {
            _ticTacToeRepository = Services.GetRequiredService<ITicTacToeRepository>();
        }

        [Fact]
        public async Task ShouldReturnNullGivenNoBoardWasFound()
        {
            // Arrange
            var fakeId = 42;
            // Act
            var board = await _ticTacToeRepository.GetBoardByItsId(fakeId);
            // Arrange
            board.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturnBoardGivenItsId()
        {
            // Arrange
            IList<Board> boards = await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .Build();
            var createdBoard = boards.First();
            // Act
            var foundBoard = await _ticTacToeRepository.GetBoardByItsId(createdBoard.Id);
            // Assert
            foundBoard.Should().Be(createdBoard);
        }

        [Fact]
        public async Task ShouldReturnSomeComputerUser()
        {
            // Arrange
            AppDbContext.AddRange(new Player { Name = "Rose", Computer = true },
                new Player { Name = "Z", Computer = true });
            await AppDbContext.SaveChangesAsync();
            // Act
            var foundBoard = await _ticTacToeRepository.GetSomeComputerPlayer();
            // Assert
            foundBoard.Computer.Should().BeTrue();
            foundBoard.Name.Should().ContainAny(new List<string> { "Rose", "Z" });
        }

        [Fact]
        public async Task ShouldSaveBoardWithItsPlayerBoards()
        {
            // Arrange
            var jafar = new Player { Name = "Jafar", Computer = false };
            var rose = new Player { Name = "Rose", Computer = true };
            AppDbContext.Players.AddRange(jafar, rose);
            await AppDbContext.SaveChangesAsync();
            var boardToBeCreated = new Board { NumberOfColumn = 3, NumberOfRows = 3 };
            var playerBoardOne = new PlayerBoard { Player = jafar, Board = boardToBeCreated };
            boardToBeCreated.PlayerBoards = new List<PlayerBoard> { playerBoardOne };
            // Act
            await _ticTacToeRepository.SaveBoard(boardToBeCreated);
            // Assert
            AppDbContext.Boards.Count().Should().Be(1);
            AppDbContext.Players.Count().Should().Be(2);
        }

        [Fact]
        public async Task ShouldCreateMovementAndRefreshBoardState()
        {
            // Arrange
            var aladdin = new Player { Name = "Aladdin", Computer = false };
            var createdBoard = (await new BoardBuilder()
                .WithDbContext(AppDbContext)
                .CreateBoard()
                .WithPlayers(aladdin)
                .Build()).First();
            var movement = new Movement { Position = 1, WhoMade = aladdin };
            // Act
            await _ticTacToeRepository.CreateMovementAndRefreshBoard(movement, createdBoard);
            // Assert
            createdBoard.Movements.Count.Should().Be(1);
        }
    }
}
