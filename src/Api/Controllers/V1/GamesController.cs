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
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IGameService _gameService;

        public GamesController(AppDbContext context, IGameService gameService)
        {
            _context = context;
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetAllGames()
        {
            Log.Information("Getting all games...");

            return await _context.Games.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetCurrentGameStatus(int id)
        {
            Log.Information("Getting specific game given ID: {Id}", id);
            var game = await _context.Games.FindAsync(id);

            if (game is null)
            {
                Log.Information("No game has been found!");
                return NotFound();
            }

            return game;
        }

        [HttpGet("play")]
        public async Task<ActionResult<GameDTO>> ApplyMovementToTheGame([FromQuery] PlayGameDto playGameDto)
        {
            Log.Information("Received PlayGameDto: {P}", playGameDto);

            var boardId = playGameDto.BoardId;
            var movementPosition = playGameDto.MovementPosition;
            var playerId = playGameDto.PlayerId;

            try
            {
                return await _gameService.ExecuteMovementAndRetrieveGameStatus(boardId, playerId, movementPosition);
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
