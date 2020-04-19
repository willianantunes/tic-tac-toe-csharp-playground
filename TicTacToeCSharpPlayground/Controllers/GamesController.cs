using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TicTacToeCSharpPlayground.Business;
using TicTacToeCSharpPlayground.Helper;
using TicTacToeCSharpPlayground.Repository;

namespace TicTacToeCSharpPlayground.Controllers
{
    [Route("tic-tac-toe/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ILogger<GamesController> _logger;
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;
        private readonly IGameDealer _gameDealer;
        private readonly CSharpPlaygroundContext _context;

        public GamesController(ILogger<GamesController> logger, ITicTacToeRepository ticTacToeRepository,
            IBoardDealer boardDealer,
            IGameDealer gameDealer, CSharpPlaygroundContext context)
        {
            _ticTacToeRepository = ticTacToeRepository;
            _boardDealer = boardDealer;
            _gameDealer = gameDealer;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAllGames()
        {
            _logger.I("Getting all games...");
            
            // TODO: Apply pagination
            return await _context.Games.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetCurrentGameStatus(Guid id)
        {
            _logger.I("Getting specific game given ID: {Id}", id);
            var game = await _context.Games.FindAsync(id);

            if (game.IsNull())
            {
                _logger.I("No game has been found!");
                return NotFound();
            }

            return game;
        }

        [HttpGet("{boardId}/{playerId}/{movementPosition}")]
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
}
