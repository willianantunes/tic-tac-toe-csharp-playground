using System;
using System.Threading.Tasks;

namespace src.Repository
{
    public interface ITicTacToeRepository
    {
        Task<Player> GetPlayerByItsId(Guid playerId);
        Task<Board> GetBoardByItsId(Guid boardId);
        Task<Game> GetGameByItsBoard(Board board);
        Task<Game> RefreshGameState(Game game);
    }
}