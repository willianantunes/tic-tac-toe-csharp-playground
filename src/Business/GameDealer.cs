using System.Threading.Tasks;
using src.Repository;

namespace src.Business
{
    public interface IGameDealer
    {
        Task<Game> GetGameByBoard(Board board);
        Task<Game> ExecuteMovementAndEvaluateResult(IGameDealer gameDealer, int movementPosition);
    }
}