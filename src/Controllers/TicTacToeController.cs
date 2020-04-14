using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using src.Business;
using src.Helper;
using src.Repository;

namespace src.Controllers
{
    [Route("tic-tac-toe")]
    [ApiController]
    public class TicTacToeController : ControllerBase
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;
        private readonly IGameDealer _gameDealer;
        private readonly ILogger<TicTacToeController> _logger;
        private readonly CSharpPlaygroundContext _context;

        public TicTacToeController(ITicTacToeRepository ticTacToeRepository, IBoardDealer boardDealer,
            IGameDealer gameDealer, ILogger<TicTacToeController> logger, CSharpPlaygroundContext context)
        {
            _ticTacToeRepository = ticTacToeRepository;
            _boardDealer = boardDealer;
            _gameDealer = gameDealer;
            _logger = logger;
            _context = context;
        }

        [HttpGet("players")]
        public async Task<ActionResult<IEnumerable<Player>>> GetAllPlayers()
        {
            // TODO: Apply pagination
            return await _context.Players.ToListAsync();
        }

        [HttpGet("players/{id}")]
        public async Task<ActionResult<Player>> GetSpecificPlayer(Guid id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player.IsNull())
                return NotFound();

            return player;
        }

        [Route("players")]
        [HttpPost]
        public async Task<ActionResult<Player>> CreateNewPlayer(Player player)
        {
            if (player.Name.IsNull())
                return BadRequest("Name is required to create a player");

            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecificPlayer", new {id = player.Id}, player);
        }

        [HttpGet("boards")]
        public async Task<ActionResult<IEnumerable<Board>>> GetAllBoards()
        {
            throw new NotImplementedException();
        }

        [HttpGet("boards/{id}")]
        public async Task<ActionResult<Board>> GetSpecificBoard(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost("boards")]
        public async Task<ActionResult<Board>> CreateNewBoard(CreateBoardDto createBoardDto)
        {
            _logger.I("Received board request object: {CreateBoardDto}", createBoardDto);

            // TODO: See a better way to apply default value
            createBoardDto.BoardSize ??= "3x3";

            if (_boardDealer.NotValidOrUnsupportedBoardSize(createBoardDto.BoardSize))
                throw new InvalidBoardConfigurationException();

            var playerOne = await _ticTacToeRepository.GetPlayerByItsId(createBoardDto.FirstPlayerId);

            if (playerOne.IsNull())
                throw new InvalidPlayerNotFoundException();

            Player playerTwo;

            if (createBoardDto.SecondPlayerId.IsNotNull())
            {
                playerTwo = await _ticTacToeRepository.GetPlayerByItsId(createBoardDto.SecondPlayerId.Value);
                if (playerTwo.IsNull())
                    throw new InvalidPlayerNotFoundException();
            }
            else
            {
                playerTwo = await _ticTacToeRepository
                    .GetSomeComputerPlayer(); // TODO: Create computer player if needed
            }

            var logMessage = "Board setup and players: {BoardSize} / {PlayerOne} / {PlayerTwo}";
            _logger.I(logMessage, createBoardDto.BoardSize, playerOne, playerTwo);
            var createdBoard = await _boardDealer.CreateNewBoard(createBoardDto.BoardSize, playerOne, playerTwo);

            return CreatedAtAction("GetSpecificBoard", new {id = createdBoard.Id}, createdBoard);
        }

        [HttpGet("games")]
        public async Task<ActionResult<IEnumerable<Game>>> GetAllGames()
        {
            throw new NotImplementedException();
        }

        [HttpGet("games/{id}")]
        public async Task<ActionResult<Game>> GetCurrentGameStatus()
        {
            throw new NotImplementedException();
        }

        [HttpGet("games/{boardId}/{playerId}/{movementPosition}")]
        public async Task<ActionResult<Game>> ApplyMovementToTheGame(Guid boardId, int movementPosition, Guid playerId)
        {
            var firstLogMessage = "Received board, movement and player: {BoardId} / {MovementPosition} / {PlayerId}";
            _logger.I(firstLogMessage, boardId, movementPosition, playerId);

            _logger.I("Searching board and player...");
            var board = await _ticTacToeRepository.GetBoardByItsId(boardId);
            if (board.IsNull())
                throw new InvalidBoardNotFoundToBePlayedException();
            // TODO player must not be a computer
            var player = await _ticTacToeRepository.GetPlayerByItsId(playerId);
            if (player.IsNull())
                throw new InvalidPlayerNotFoundException();

            _logger.I("Searching for a game...");
            var game = await _gameDealer.GetGameByBoard(board);
            if (game.IsFinished())
                throw new InvalidGameIsNotPlayableAnymoreException();

            if (_boardDealer.PositionIsNotAvailable(game.ConfiguredBoard, movementPosition))
            {
                IList<int> position = _boardDealer.AvailablePositions(game.ConfiguredBoard);
                return BadRequest($"Available positions: {position}");
            }

            _logger.I("Executing movement and evaluating game...");
            var evaluatedGame = await _gameDealer.ExecuteMovementAndEvaluateResult(game, movementPosition, player);

            if (evaluatedGame.IsFinished())
                _logger.I("Game conclusion: {EvaluatedGame}", evaluatedGame);
            else
                _logger.I("Game hasn't finished yet!");

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
