using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Exceptions;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Core.Services;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Board> _databaseSet;
        private readonly IGameService _gameService;

        public BoardsController(AppDbContext context, IGameService gameService)
        {
            _context = context;
            _databaseSet = context.Boards;
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> GetAllBoards()
        {
            Log.Information("Getting all boards...");

            return await _databaseSet.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetSpecificBoard(int id)
        {
            Log.Information("Getting specific board given ID: {Id}", id);
            var board = await _databaseSet.FindAsync(id);

            if (board is null)
            {
                Log.Information("No board has been found!");
                return NotFound();
            }

            return board;
        }

        [HttpPost]
        public async Task<ActionResult<BoardDTO>> CreateNewBoard([FromBody] CreateBoardDto createBoardDto)
        {
            Log.Information("Board to be created: {CreateBoardDto}", createBoardDto);

            var boardSize = createBoardDto.BoardSize;
            var firstPlayerId = createBoardDto.FirstPlayerId;
            var secondPlayerId = createBoardDto.SecondPlayerId;
            
            try
            {
                var board = await _gameService.CreateNewBoard(boardSize, firstPlayerId, secondPlayerId);
                return CreatedAtAction("GetSpecificBoard", new {id = board.Id}, board);
            }
            catch (TicTacToeRequiredDataExceptions requiredDataExcep)
            {
                var message = requiredDataExcep.Message;
                throw new HttpException {StatusCode = (int) HttpStatusCode.NotFound, Details = message};
            }
            catch (TicTacToeContractExceptions contractExcep)
            {
                var message = contractExcep.Message;
                throw new HttpException {StatusCode = (int) HttpStatusCode.BadRequest, Details = message};
            }
        }
    }
}
