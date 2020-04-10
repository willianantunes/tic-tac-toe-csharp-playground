using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using src.Business;
using src.Helper;
using src.Repository;

namespace src.Controllers
{
    public class TicTacToeController : ControllerBase
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;
        private readonly IGameDealer _gameDealer;
        private readonly ILogger<TicTacToeController> logger;

        public void GetAllPlayers()
        {
            // TODO
        }

        public void GetSpecificPlayer()
        {
            // TODO
        }

        public void CreateNewPlayer()
        {
            // TODO
        }

        public void GetAllBoards()
        {
            // TODO
        }

        public void GetSpecificBoard()
        {
            // TODO
        }

        public async Task<ActionResult<Board>> CreateNewBoard(string? boardSize, Guid playerId)
        {
            logger.Info("Received board and playerId: {BoardSize} / {PlayerId}", boardSize, playerId);

            if (_boardDealer.NotValidOrUnsupportedBoardSize(boardSize))
                throw new InvalidBoardConfigurationException();

            var player = await _ticTacToeRepository.GetPlayerByItsId(playerId);

            if (player is null)
                throw new InvalidPlayerNotFoundException();

            logger.Info("Board setup and player: {BoardSize} / {Player}", boardSize, player);
            var createdBoard = await _boardDealer.CreateNewBoard(boardSize, player);

            return CreatedAtAction("GetSpecificBoard", new {id = createdBoard.Id}, createdBoard);
        }

        public void GetAllGames()
        {
            // TODO
        }

        public void GetCurrentGameStatus()
        {
            // TODO
        }

        public async Task<ActionResult<Game>> ApplyMovementToTheGame(Guid boardId, int movementPosition)
        {
            logger.Info("Received board and movement: {BoardId} / {MovementPosition}", boardId, movementPosition);
            var board = await _ticTacToeRepository.GetBoardByItsId(boardId);

            if (board is null)
                throw new InvalidBoardNotFoundToBePlayedException();

            logger.Info("Searching for a game");
            var game = await _gameDealer.GetGameByBoard(board);

            if (game.isFinished())
                throw new InvalidBoardIsNotPlayableAnymoreException();
            if (game.PositionIsNotAvailable(movementPosition))
            {
                IList<int> position = game.AvailablePositions();
                return BadRequest($"Available positions: {position}");
            }

            logger.Info("Executing movement and evaluating game...");
            var evaluatedGame = await _gameDealer.ExecuteMovementAndEvaluateResult(_gameDealer, movementPosition);

            logger.Info("Evaluated game: {EvaluatedGame}", evaluatedGame);
            return evaluatedGame;
        }
    }

    #region Exceptions

    public class InvalidBoardIsNotPlayableAnymoreException : Exception
    {
    }

    public class InvalidBoardNotFoundToBePlayedException : Exception
    {
    }

    public class InvalidPlayerNotFoundException : Exception
    {
    }

    public class InvalidBoardConfigurationException : Exception
    {
    }

    #endregion
}