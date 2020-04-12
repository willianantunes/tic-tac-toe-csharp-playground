using System.Linq;
using System.Threading.Tasks;
using src.Repository;

namespace src.Business
{
    public interface IGameDealer
    {
        Task<Game> GetGameByBoard(Board board);
        Task<Game> ExecuteMovementAndEvaluateResult(Game game, int movementPosition, Player player);
    }

    public class GameDealer : IGameDealer
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;

        public GameDealer()
        {
        }
        
        public GameDealer(ITicTacToeRepository ticTacToeRepository, IBoardDealer boardDealer)
        {
            _ticTacToeRepository = ticTacToeRepository;
            _boardDealer = boardDealer;
        }

        public async Task<Game> GetGameByBoard(Board board)
        {
            Game game = await _ticTacToeRepository.GetGameByItsBoard(board);

            if (game is null)
                game = new Game(board);

            _boardDealer.InitializeBoardConfiguration(board);

            return game;
        }

        public Task<Game> ExecuteMovementAndEvaluateResult(Game game, int movementPosition, Player player)
        {
            var gameConfiguredBoard = game.ConfiguredBoard;
            _boardDealer.ApplyMovement(gameConfiguredBoard, movementPosition, player);
            BoardSituation boardSituation = _boardDealer.EvaluateTheSituation(gameConfiguredBoard, movementPosition);

            if (boardSituation.HasAWinner || boardSituation.SadlyFinishedWithDraw)
            {
                UpdateGameGivenItsResult(game, boardSituation);
                return _ticTacToeRepository.RefreshGameState(game);
            }
            if (_boardDealer.AvailablePositions(gameConfiguredBoard).Count <= 0)
                return _ticTacToeRepository.RefreshGameState(game);
            
            var executed = ExecuteComputerMovementIfApplicable(player, gameConfiguredBoard);
            if (executed)
            {
                boardSituation = _boardDealer.EvaluateTheSituation(gameConfiguredBoard, movementPosition);
                UpdateGameGivenItsResult(game, boardSituation);    
            }
            

            return _ticTacToeRepository.RefreshGameState(game);
        }

        private bool ExecuteComputerMovementIfApplicable(Player player, Board gameConfiguredBoard)
        {
            if (player.isNotComputer() && _boardDealer.HasComputerPlayer(gameConfiguredBoard))
            {
                var somePosition = _boardDealer.AvailablePositions(gameConfiguredBoard).First();
                _boardDealer.ApplyMovementForComputer(gameConfiguredBoard, somePosition);
                return true;
            }

            return false;
        }

        private static void UpdateGameGivenItsResult(Game game, BoardSituation boardSituation)
        {
            if (boardSituation.SadlyFinishedWithDraw)
            {
                game.Draw = true;
                game.Finished = true;
            }
            else if (boardSituation.HasAWinner)
            {
                game.Winner = boardSituation.Winner;
                game.Finished = true;
            }
        }
    }
}
