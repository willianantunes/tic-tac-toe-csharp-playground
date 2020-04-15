using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        Task<Board> CreateMovementAndRefreshBoard(Movement movement, Board board);
    }

    public class TicTacToeRepository : ITicTacToeRepository
    {
        private CSharpPlaygroundContext _playgroundContext;

        public TicTacToeRepository(CSharpPlaygroundContext playgroundContext)
        {
            _playgroundContext = playgroundContext;
        }

        public async ValueTask<Player> GetPlayerByItsId(Guid playerId)
        {
            return await _playgroundContext.Players.FindAsync(playerId);
        }

        public async ValueTask<Board> GetBoardByItsId(Guid boardId)
        {
            var boards = await _playgroundContext.Boards
                .Where(b => b.Id == boardId)
                .Include(b => b.PlayerBoards)
                .ThenInclude(pb => pb.Player)
                .ToListAsync();

            return boards.FirstOrDefault();
        }

        public async Task<Game> GetGameByItsBoard(Board board)
        {
            Expression<Func<Game, bool>> predicate = game => game.ConfiguredBoard.Id == board.Id;

            var games = await _playgroundContext.Games
                .Where(game => game.ConfiguredBoard.Id == board.Id)
                .Include(g => g.ConfiguredBoard)
                .ThenInclude(b => b.Movements)
                .ToListAsync();

            return games.FirstOrDefault();
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

        public async Task<Board> CreateMovementAndRefreshBoard(Movement movement, Board board)
        {
            if (board.Movements.IsNull())
                board.Movements = new List<Movement>();

            board.Movements.Add(movement);

            var entityEntryBoard = _playgroundContext.Boards.Update(board);
            await _playgroundContext.SaveChangesAsync();
            
            return entityEntryBoard.Entity;
        }
    }
}
