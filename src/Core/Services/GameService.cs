using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Serilog;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Exceptions;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Repository;

namespace TicTacToeCSharpPlayground.Core.Services
{
    public interface IGameService
    {
        Task<BoardDTO> CreateNewBoard(string boardSize, int firstPlayerId, int secondPlayerId);
        Task<GameDTO> ExecuteMovementAndRetrieveGameStatus(int boardId, int playerId, int movementPosition);
    }

    public class GameService : IGameService
    {
        private readonly IMapper _mapper;
        private readonly IBoardDealer _boardDealer;
        private readonly IPositionDecider _positionDecider;
        private readonly ITicTacToeRepository _repository;

        public GameService(IMapper mapper, IBoardDealer boardDealer, IPositionDecider positionDecider, ITicTacToeRepository repository)
        {
            _mapper = mapper;
            _boardDealer = boardDealer;
            _positionDecider = positionDecider;
            _repository = repository;
        }

        public async Task<BoardDTO> CreateNewBoard(string boardSize, int firstPlayerId, int secondPlayerId)
        {
            Log.Information("Checking board setup");
            if (_boardDealer.NotValidOrUnsupportedBoardSize(boardSize))
            {
                var message = $"Board {boardSize} is not supported. You can try 3x3 👍";
                throw new InvalidBoardConfigurationException(message);
            }

            Log.Information("Checking players");
            var playerOne = await _repository.GetPlayerByItsId(firstPlayerId);
            var playerTwo = await _repository.GetPlayerByItsId(secondPlayerId);
            if (playerOne is null || playerTwo is null)
            {
                var p1 = playerOne?.Name is null ? "❓" : playerOne.Name;
                var p2 = playerTwo?.Name is null ? "❓" : playerTwo.Name;
                var message = $"Both players are required. P1: {p1} | P2: {p2}";
                throw new PlayerNotFoundException(message);
            }

            Log.Information("Creating board");
            var freshNewBoard = _boardDealer.PrepareBoardWithRequestSetup(boardSize, playerOne, playerTwo);
            await _repository.SaveBoard(freshNewBoard);

            return _mapper.Map<Board, BoardDTO>(freshNewBoard);
        }

        public async Task<GameDTO> ExecuteMovementAndRetrieveGameStatus(int boardId, int playerId, int movementPosition)
        {
            Log.Information("Searching the board 🎮");
            var board = await _repository.GetBoardByItsId(boardId);
            if (board is null)
            {
                var message = $"The board {boardId} is not available. Are you sure you are correct? 🤔";
                throw new BoardNotFoundToBePlayedException(message);
            }

            Log.Information("Searching the user 🕹");
            var player = await _repository.GetPlayerByItsId(playerId);
            if (player is null)
                throw new PlayerNotFoundException($"There is no player with ID {playerId}");
            if (player.Computer)
                throw new YouAreNotAllowedToPlayWithARobotException($"{player.Name} is a robot. Only I can use it!");

            Log.Information("Searching for a game 🎰");
            var game = await _repository.GetGameByItsBoard(board);
            game ??= new Game(board);
            if (game.IsFinished())
                throw new GameIsNotPlayableException($"The game associated with the board {board.Id} is finished");

            Log.Information("Checking position 🕵");
            if (board.PositionIsNotAvailable(movementPosition))
            {
                IList<int> freePositions = board.FreeFields;
                var positions = String.Join(" ", freePositions);
                var message = $"Position {movementPosition} is not available. The ones you can choose: {positions}";
                throw new PositionNotAvailableException(message);
            }

            var result = await ExecuteMovementAndRetrieveResult(movementPosition, player, board);
            if (result.IsGameOver is false)
            {
                var robotPlayer = board.GetRobotPlayer();
                if (robotPlayer is not null)
                {
                    int position = _positionDecider.ChooseTheBestAvailablePositionFor(board.FreeFields);
                    result = await ExecuteMovementAndRetrieveResult(position, robotPlayer, board);
                }
            }

            Log.Information("Updating game state 📝");
            game.Draw = result.IsDraw;
            game.Finished = result.IsGameOver;
            game.Winner = result.IsGameOver && result.HasAWinner ? result.WhoDidTheLastMovement : null;
            await _repository.RefreshGameState(game);

            return _mapper.Map<Game, GameDTO>(game);
        }

        private async Task<MovementResult> ExecuteMovementAndRetrieveResult(int movementPosition, Player player, Board board)
        {
            Log.Information($"Executing movement for player {player.Name} and evaluating game 🔍");

            var createdMovement = _boardDealer.CreateMovementForCustomPlayerOrComputer(board, movementPosition, player);
            await _repository.CreateMovementAndRefreshBoard(createdMovement, board);
            var (hasAWinner, isDraw) = _boardDealer.EvaluateTheSituation(board, movementPosition);
            var noMoreMovementsAvailable = board.FreeFields.Count <= 0;
            var isGameOver = hasAWinner || isDraw || noMoreMovementsAvailable;

            return new MovementResult(hasAWinner, isDraw, noMoreMovementsAvailable, isGameOver, player);
        }

        public record MovementResult(bool HasAWinner, bool IsDraw, bool NoMoreMovementsAvailable, bool IsGameOver, Player WhoDidTheLastMovement);
    }
}
