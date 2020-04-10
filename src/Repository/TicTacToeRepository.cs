using System;
using System.Threading.Tasks;

namespace src.Repository
{
    public interface ITicTacToeRepository
    {
        Task<Player> GetPlayerByItsId(Guid playerId);
        Task<Board> GetBoardByItsId(Guid boardId);
    }
}