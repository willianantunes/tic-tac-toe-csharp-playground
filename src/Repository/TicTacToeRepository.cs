using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using src.Helper;

namespace src.Repository
{
    public interface ITicTacToeRepository
    {
        ValueTask<Player> GetPlayerByItsId(Guid playerId);
        ValueTask<Board> GetBoardByItsId(Guid boardId);
        Task<Game> GetGameByItsBoard(Board board);
        Task<Game> RefreshGameState(Game game);
        Task<Player> GetSomeComputerPlayer();
        Task SaveBoard(Board board);
        Task CreateMovementAndRefreshBoard(Movement movement, Board board);
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

        public async Task<Player> GetSomeComputerPlayer()
        {
            return await _playgroundContext.Players.FirstAsync(p => p.Computer);
        }

        public async Task SaveBoard(Board board)
        {
            foreach (var boardPlayerBoard in board.PlayerBoards)
            {
                // TODO: Proposed solution: https://stackoverflow.com/a/39165051/3899136
                // But I'll study a better way to do it
                var p = await _playgroundContext.Players.FindAsync(boardPlayerBoard.Player.Id);
                boardPlayerBoard.Player = p;
                boardPlayerBoard.Id = Guid.NewGuid();
            }

            _playgroundContext.Boards.Add(board);
            await _playgroundContext.SaveChangesAsync();
        }

        public async Task CreateMovementAndRefreshBoard(Movement movement, Board board)
        {
            _playgroundContext.Movements.Add(movement);
            await _playgroundContext.SaveChangesAsync();

            if (board.Movements.IsNull())
                board.Movements = new List<Movement>();

            board.Movements.Add(movement);

            _playgroundContext.Boards.Update(board);
            await _playgroundContext.SaveChangesAsync();
        }
    }
}
