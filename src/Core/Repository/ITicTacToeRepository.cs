using System.Threading.Tasks;
using TicTacToeCSharpPlayground.Core.Models;

namespace TicTacToeCSharpPlayground.Core.Repository
{
    public interface ITicTacToeRepository
    {
        Task<Player?> GetPlayerByItsId(int playerId);
        Task<Board?> GetBoardByItsId(int boardId);
        Task<Game?> GetGameByItsBoard(Board board);
        Task<Game> RefreshGameState(Game game);
        Task<Player> GetSomeComputerPlayer();
        Task SaveBoard(Board board);
        Task<Board> CreateMovementAndRefreshBoard(Movement movement, Board board);
    }
}
