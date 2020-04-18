using System;
using System.Linq;
using System.Threading.Tasks;
using TicTacToeCSharpPlayground.Repository;

namespace TicTacToeCSharpPlayground.Business
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

        public async Task<Game> ExecuteMovementAndEvaluateResult(Game game, int movementPosition, Player player)
        {
            var configuredBoard = game.ConfiguredBoard;
            await _boardDealer.ApplyMovement(configuredBoard, movementPosition, player);
            BoardSituation boardSituation = _boardDealer.EvaluateTheSituation(configuredBoard, movementPosition);

            if (boardSituation.HasAWinner || boardSituation.SadlyFinishedWithDraw)
            {
                // TODO: TEMP
                boardSituation.Winner = player;
                UpdateGameGivenItsResult(game, boardSituation);
                return await _ticTacToeRepository.RefreshGameState(game);
            }
            if (_boardDealer.AvailablePositions(configuredBoard).Count <= 0)
                return await _ticTacToeRepository.RefreshGameState(game);
            
            var executed = await ExecuteComputerMovementIfApplicable(player, configuredBoard);
            if (executed)
            {
                boardSituation = _boardDealer.EvaluateTheSituation(configuredBoard, movementPosition);
                UpdateGameGivenItsResult(game, boardSituation);    
            }
            

            return await _ticTacToeRepository.RefreshGameState(game);
        }

        private async Task<bool> ExecuteComputerMovementIfApplicable(Player player, Board gameConfiguredBoard)
        {
            // TODO: REFACTORY
            if (player.isNotComputer() && _boardDealer.HasComputerPlayer(gameConfiguredBoard))
            {
                var somePosition = _boardDealer.AvailablePositions(gameConfiguredBoard).First();
                // TODO: REFACTORY
                Func<PlayerBoard, bool> predicate = pb => !pb.Player.isNotComputer();
                var computerPlayer = gameConfiguredBoard.PlayerBoards.First(predicate);
                
                await _boardDealer.ApplyMovement(gameConfiguredBoard, somePosition, computerPlayer.Player);
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
