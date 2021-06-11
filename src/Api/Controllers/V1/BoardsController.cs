using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;
using TicTacToeCSharpPlayground.Infrastructure.Database.Repositories;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    [Route("tic-tac-toe/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly ITicTacToeRepository _ticTacToeRepository;
        private readonly IBoardDealer _boardDealer;
        private readonly AppDbContext _context;

        public BoardsController(ITicTacToeRepository ticTacToeRepository,
            IBoardDealer boardDealer,
            AppDbContext context)
        {
            _ticTacToeRepository = ticTacToeRepository;
            _boardDealer = boardDealer;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetAllBoards()
        {
            Log.Information("Getting all boards...");
            
            // TODO: Apply pagination
            return await _context.Boards.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetSpecificBoard(Guid id)
        {
            Log.Information("Getting specific board given ID: {Id}", id);
            var board = await _context.Boards.FindAsync(id);

            if (board is null)
            {
                Log.Information("No board has been found!");
                return NotFound();
            }

            return board;
        }

        [HttpPost]
        public async Task<ActionResult<Board>> CreateNewBoard(CreateBoardDto createBoardDto)
        {
            Log.Information("Received board request object: {CreateBoardDto}", createBoardDto);

            // TODO: See a better way to apply default value
            createBoardDto.BoardSize ??= "3x3";

            if (_boardDealer.NotValidOrUnsupportedBoardSize(createBoardDto.BoardSize))
                throw new InvalidBoardConfigurationException();

            var playerOne = await _ticTacToeRepository.GetPlayerByItsId(createBoardDto.FirstPlayerId);

            if (playerOne is null)
                throw new InvalidPlayerNotFoundException();

            Player playerTwo;

            if (createBoardDto.SecondPlayerId is not null)
            {
                playerTwo = await _ticTacToeRepository.GetPlayerByItsId(createBoardDto.SecondPlayerId.Value);
                if (playerTwo is null)
                    throw new InvalidPlayerNotFoundException();
            }
            else
            {
                playerTwo = await _ticTacToeRepository
                    .GetSomeComputerPlayer(); // TODO: Create computer player if needed
            }

            var logMessage = "Board setup and players: {BoardSize} / {PlayerOne} / {PlayerTwo}";
            Log.Information(logMessage, createBoardDto.BoardSize, playerOne, playerTwo);
            var createdBoard = await _boardDealer.CreateNewBoard(createBoardDto.BoardSize, playerOne, playerTwo);

            return CreatedAtAction("GetSpecificBoard", new {id = createdBoard.Id}, createdBoard);
        }
    }
}
