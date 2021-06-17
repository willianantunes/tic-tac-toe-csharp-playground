using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Repository;

namespace TicTacToeCSharpPlayground.Infrastructure.Database.Repositories
{
    public class TicTacToeRepository : ITicTacToeRepository
    {
        private AppDbContext _appDbContext;

        public TicTacToeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Player?> GetPlayerByItsId(int playerId)
        {
            return await _appDbContext.Players.FindAsync(playerId);
        }

        public async Task<Board?> GetBoardByItsId(int boardId)
        {
            var boards = await _appDbContext.Boards
                .Where(b => b.Id == boardId)
                .Include(b => b.PlayerBoards)
                .ThenInclude(pb => pb.Player)
                .ToListAsync();

            return boards.FirstOrDefault();
        }

        public async Task<Game?> GetGameByItsBoard(Board board)
        {
            var games = await _appDbContext.Games
                .Where(game => game.ConfiguredBoard.Id == board.Id)
                .Include(g => g.ConfiguredBoard)
                .ThenInclude(b => b.Movements)
                .ToListAsync();
            var possibleGame = games.FirstOrDefault();
            // To refresh board status
            possibleGame?.ConfiguredBoard?.InitializeBoardConfiguration();
            return possibleGame;
        }

        public async Task<Game> RefreshGameState(Game game)
        {
            // https://docs.microsoft.com/en-us/ef/core/saving/basic#updating-data
            var entityEntry = _appDbContext.Games.Update(game);
            // TODO: Maybe disable AutoDetectChangesEnabled as it is enabled by default;
            var stateEntriesWrittenToTheDatabase = await _appDbContext.SaveChangesAsync();
            // TODO: Maybe log stateEntriesWrittenToTheDatabase?
            return entityEntry.Entity;
        }

        public async Task<Player> GetSomeComputerPlayer()
        {
            return await _appDbContext.Players.FirstAsync(p => p.Computer);
        }

        public async Task SaveBoard(Board board)
        {
            foreach (var boardPlayerBoard in board.PlayerBoards)
            {
                // TODO: Proposed solution: https://stackoverflow.com/a/39165051/3899136
                var p = await _appDbContext.Players.FindAsync(boardPlayerBoard.Player.Id);
                boardPlayerBoard.Player = p;
            }
            _appDbContext.Boards.Add(board);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<Board> CreateMovementAndRefreshBoard(Movement movement, Board board)
        {
            if (board.Movements is null)
                board.Movements = new List<Movement>();

            board.Movements.Add(movement);

            var entityEntryBoard = _appDbContext.Boards.Update(board);
            await _appDbContext.SaveChangesAsync();

            return entityEntryBoard.Entity;
        }
    }
}
