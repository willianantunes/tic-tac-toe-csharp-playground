using System.Threading.Tasks;
using src.Repository;

namespace src.Business
{
    public interface IBoardDealer
    {
        bool NotValidOrUnsupportedBoardSize(string? boardSize);
        Task<Board> CreateNewBoard(string? boardSize, Player player);
    }
}