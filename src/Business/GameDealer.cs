using System.Threading.Tasks;
using src.Repository;

namespace src.Business
{
    public interface IGameDealer
    {
        Task<Game> GetGameByBoard(Board board);
        Task<Game> ExecuteMovementAndEvaluateResult(Game gameDealer, int movementPosition);
    }

    class GameDealer : IGameDealer
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;

        public async Task<Game> GetGameByBoard(Board board)
        {
            Game game = await _ticTacToeRepository.GetGameByItsBoard(board);

            if (game is null)
                game = new Game(board);

            return game;
        }

        public Task<Game> ExecuteMovementAndEvaluateResult(Game gameDealer, int movementPosition)
        {
            var movement = new Movement();
            movement.Position = movementPosition;
            gameDealer.Movements.Add(movement);
            
            throw new System.NotImplementedException();
        }
    }
}