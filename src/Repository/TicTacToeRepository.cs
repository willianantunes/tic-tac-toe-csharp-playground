using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace src.Repository
{
    public interface ITicTacToeRepository
    {
        ValueTask<Player> GetPlayerByItsId(Guid playerId);
        ValueTask<Board> GetBoardByItsId(Guid boardId);
        Task<Game> GetGameByItsBoard(Board board);
        Task<Game> RefreshGameState(Game game);
    }

    public class TicTacToeRepository : ITicTacToeRepository
    {
        private CSharpPlaygroundContext _playgroundContext;

        public TicTacToeRepository(CSharpPlaygroundContext playgroundContext)
        {
            _playgroundContext = playgroundContext;
        }

        public ValueTask<Player> GetPlayerByItsId(Guid playerId)
        {
            return _playgroundContext.Players.FindAsync(playerId);
        }

        public ValueTask<Board> GetBoardByItsId(Guid boardId)
        {
            return _playgroundContext.Boards.FindAsync(boardId);
        }

        public Task<Game> GetGameByItsBoard(Board board)
        {
            return _playgroundContext.Games.SingleAsync(game => game.ConfiguredBoard.Id == board.Id);
        }

        public async Task<Game> RefreshGameState(Game game)
        {
            // https://docs.microsoft.com/en-us/ef/core/saving/basic#updating-data
            var entityEntry = _playgroundContext.Games.Update(game);
            // TODO: Maybe disable AutoDetectChangesEnabled as it is enabled by default;
            var stateEntriesWrittenToTheDatabase = await _playgroundContext.SaveChangesAsync();
            // TODO: Maybe log stateEntriesWrittenToTheDatabase?
            return entityEntry.Entity;
        }
    }
}
