using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using DrfLikePaginations;
using Microsoft.AspNetCore.Http.Extensions;
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
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;
        private readonly IPagination _pagination;

        public BoardsController(AppDbContext context, IGameService gameService, IPagination pagination, IMapper mapper)
        {
            _context = context;
            _gameService = gameService;
            _pagination = pagination;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Paginated<BoardDTO>>> GetAllBoards()
        {
            Log.Information("Getting all boards...");
            var query = _context.Boards.AsNoTracking().OrderBy(b => b.CreatedAt);
            var displayUrl = Request.GetDisplayUrl();
            var queryParams = Request.Query;
            Func<Board, BoardDTO> transform = p => _mapper.Map<Board, BoardDTO>(p);

            return await _pagination.CreateAsync(query, displayUrl, queryParams, transform);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetSpecificBoard(int id)
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
        public async Task<ActionResult<BoardDTO>> CreateNewBoard([FromBody] CreateBoardDto createBoardDto)
        {
            Log.Information("Board to be created: {CreateBoardDto}", createBoardDto);

            var boardSize = createBoardDto.BoardSize;
            var firstPlayerId = createBoardDto.FirstPlayerId;
            var secondPlayerId = createBoardDto.SecondPlayerId;

            try
            {
                var board = await _gameService.CreateNewBoard(boardSize, firstPlayerId, secondPlayerId);
                return CreatedAtAction("GetSpecificBoard", new { id = board.Id }, board);
            }
            catch (TicTacToeRequiredDataExceptions requiredDataExcep)
            {
                var message = requiredDataExcep.Message;
                throw new HttpException { StatusCode = (int)HttpStatusCode.NotFound, Details = message };
            }
            catch (TicTacToeContractExceptions contractExcep)
            {
                var message = contractExcep.Message;
                throw new HttpException { StatusCode = (int)HttpStatusCode.BadRequest, Details = message };
            }
        }
    }
}
