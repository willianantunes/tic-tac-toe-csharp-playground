using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using TicTacToeCSharpPlayground.Business;
using TicTacToeCSharpPlayground.Repository;
using Xunit;

namespace tests.Unit.Business
{
    public class GameDealerTest
    {
        private readonly Mock<ITicTacToeRepository> _ticTacToeRepository;
        private readonly Mock<IBoardDealer> _boardDealer;
        private readonly GameDealer _gameDealer;

        public GameDealerTest()
        {
            _ticTacToeRepository = new Mock<ITicTacToeRepository>();
            _boardDealer = new Mock<IBoardDealer>();
            _gameDealer = new GameDealer(_ticTacToeRepository.Object, _boardDealer.Object);
        }

        [Fact]
        public async Task ShouldCreateNewGameAsNoGameWasFoundAndConfigureBoard()
        {
            var board = new Board();
            _ticTacToeRepository
                .Setup(r => r.GetGameByItsBoard(It.Is<Board>(p => p == board)))
                .ReturnsAsync(() => null);

            var game = await _gameDealer.GetGameByBoard(board);

            game.ConfiguredBoard.Should().Equals(board);
            _ticTacToeRepository.Verify();
            _boardDealer.Verify(b => b.InitializeBoardConfiguration(board), Times.Once);
        }
        
        [Fact]
        public async Task ShouldReturnPreviouslyCreatedGameAsItWasFoundAndConfigureBoard()
        {
            var board = new Board();
            var gameToBeRetrieved = new Game();
            gameToBeRetrieved.ConfiguredBoard = board;
            _ticTacToeRepository
                .Setup(r => r.GetGameByItsBoard(It.Is<Board>(p => p == board)))
                .ReturnsAsync(() => gameToBeRetrieved);

            var game = await _gameDealer.GetGameByBoard(board);

            game.Should().BeSameAs(gameToBeRetrieved);
            game.ConfiguredBoard.Should().Equals(board);
            _ticTacToeRepository.Verify();
            _boardDealer.Verify(b => b.InitializeBoardConfiguration(board), Times.Once);
        }
    }
}
