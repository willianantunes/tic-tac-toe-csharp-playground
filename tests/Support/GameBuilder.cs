using System.Threading.Tasks;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace Tests.Support
{
    public class GameBuilder
    {
        private Board _board;
        private Player _playerTwo;
        private Player _playerOne;
        private Game _game;
        private AppDbContext _dbContext;

        public GameBuilder WithDbContext(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            return this;
        }

        public GameBuilder WithBoard(Board createdBoard)
        {
            _board = createdBoard;
            return this;
        }

        public GameBuilder WithPlayers(Player playerOne, Player playerTwo)
        {
            _playerOne = playerOne;
            _playerTwo = playerTwo;

            return this;
        }

        public GameBuilder PlayerOneWinning()
        {
            _game = new Game()
            {
                Draw = false,
                Finished = true,
                Winner = _playerOne,
                ConfiguredBoard = _board
            };

            return this;
        }

        public async Task<Game> Build(bool clearOldData = true)
        {
            var entityEntry = await _dbContext.Games.AddAsync(_game);

            await _dbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }
    }
}
