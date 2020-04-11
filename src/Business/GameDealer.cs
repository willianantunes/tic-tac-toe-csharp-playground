using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.Helper;
using src.Repository;

namespace src.Business
{
    public interface IGameDealer
    {
        Task<Game> GetGameByBoard(Board board);
        Task<Game> ExecuteMovementAndEvaluateResult(Game game, int movementPosition, Player player);
        bool PositionIsNotAvailable(Game game, in int movementPosition);
        IList<int> AvailablePositions(Game game);
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

        public Task<Game> ExecuteMovementAndEvaluateResult(Game game, int movementPosition, Player player)
        {
            var movement = new Movement();
            movement.Position = movementPosition;
            game.ConfiguredBoard.Movements.Add(movement);

            throw new System.NotImplementedException();
        }

        public bool PositionIsNotAvailable(Game game, in int movementPosition)
        {
            var copiedMovementPosition = movementPosition;
            var foundMovement = game.ConfiguredBoard.Movements.First(m => m.Position.Equals(copiedMovementPosition));

            if (foundMovement.IsNotNull())
                return true;

            return AvailablePositions(game).Contains(copiedMovementPosition);
        }

        public IList<int> AvailablePositions(Game game)
        {
            var gameConfiguredBoard = game.ConfiguredBoard;
            var availablePosition = new List<int>();
            var positionCount = 1;
            
            for (int column = 1; column <= gameConfiguredBoard.NumberOfColumn; column++)
            {
                for (int row = 1; row <= gameConfiguredBoard.NumberOfRows; row++)
                {
                    var hasNoPosition = gameConfiguredBoard.Movements.None(m => m.Position == positionCount);

                    if (hasNoPosition)
                        availablePosition.Add(positionCount);

                    positionCount++;
                }
            }

            return availablePosition;
        }
    }
}