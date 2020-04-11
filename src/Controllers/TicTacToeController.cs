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

        public async Task<ActionResult<Board>> CreateNewBoard(string? boardSize, Guid firstPlayerId,
            Guid? secondPlayerId)
        {
            var logMessage = "Received board and players: {BoardSize} / {FirstPlayer} / {SecondPlayer}";
            logger.I(logMessage, boardSize, firstPlayerId, secondPlayerId);

            if (_boardDealer.NotValidOrUnsupportedBoardSize(boardSize))
                throw new InvalidBoardConfigurationException();

            var playerOne = await _ticTacToeRepository.GetPlayerByItsId(firstPlayerId);

            if (playerOne.IsNull())
                throw new InvalidPlayerNotFoundException();

            var playerTwo = await _ticTacToeRepository.GetPlayerByItsId(firstPlayerId);

            logMessage = "Board setup and players: {BoardSize} / {PlayerOne} / {PlayerTwo}";
            logger.I(logMessage, boardSize, playerOne, playerTwo);
            var createdBoard = await _boardDealer.CreateNewBoard(boardSize, playerOne, playerTwo);

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

        public async Task<ActionResult<Game>> ApplyMovementToTheGame(Guid boardId, int movementPosition, Guid playerId)
        {
            var firstLogMessage = "Received board, movement and player: {BoardId} / {MovementPosition} / {PlayerId}";
            logger.I(firstLogMessage, boardId, movementPosition, playerId);

            logger.I("Searching board and player...");
            var board = await _ticTacToeRepository.GetBoardByItsId(boardId);
            if (board.IsNull())
                throw new InvalidBoardNotFoundToBePlayedException();
            // TODO player must not be a computer
            var player = await _ticTacToeRepository.GetPlayerByItsId(playerId);
            if (player.IsNull())
                throw new InvalidPlayerNotFoundException();

            logger.I("Searching for a game...");
            var game = await _gameDealer.GetGameByBoard(board);
            if (game.IsFinished())
                throw new InvalidGameIsNotPlayableAnymoreException();

            if (_boardDealer.PositionIsNotAvailable(game.ConfiguredBoard, movementPosition))
            {
                IList<int> position = _boardDealer.AvailablePositions(game.ConfiguredBoard);
                return BadRequest($"Available positions: {position}");
            }

            logger.I("Executing movement and evaluating game...");
            var evaluatedGame = await _gameDealer.ExecuteMovementAndEvaluateResult(game, movementPosition, player);

            if (evaluatedGame.IsFinished())
                logger.I("Game conclusion: {EvaluatedGame}", evaluatedGame);
            else
                logger.I("Game hasn't finished yet!");

            return evaluatedGame;
        }
    }

    #region Exceptions

    public class InvalidGameIsNotPlayableAnymoreException : Exception
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