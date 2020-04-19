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
    public class BoardsController : ControllerBase
    {
        private readonly ILogger<BoardsController> _logger;
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;
        private readonly CSharpPlaygroundContext _context;

        public BoardsController(ILogger<BoardsController> logger, ITicTacToeRepository ticTacToeRepository,
            IBoardDealer boardDealer,
            CSharpPlaygroundContext context)
        {
            _ticTacToeRepository = ticTacToeRepository;
            _boardDealer = boardDealer;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetAllBoards()
        {
            _logger.I("Getting all boards...");
            
            // TODO: Apply pagination
            return await _context.Boards.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetSpecificBoard(Guid id)
        {
            _logger.I("Getting specific board given ID: {Id}", id);
            var board = await _context.Boards.FindAsync(id);

            if (board.IsNull())
            {
                _logger.I("No board has been found!");
                return NotFound();
            }

            return board;
        }

        [HttpPost]
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
    }
}
