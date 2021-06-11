using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PlayersController(AppDbContext context, ILogger<PlayersController> logger)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetAllPlayers()
        {
            Log.Information("Getting all players...");

            // TODO: Apply pagination
            return await _context.Players.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetSpecificPlayer(Guid id)
        {
            Log.Information("Getting specific player given ID: {Id}", id);
            var player = await _context.Players.FindAsync(id);

            if (player is null)
            {
                Log.Information("No player has been found!");
                return NotFound();
            }

            return player;
        }

        [HttpPost]
        public async Task<ActionResult<Player>> CreateNewPlayer(Player? player)
        {
            if (player?.Name is null)
                return BadRequest("Name is required to create a player");

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecificPlayer", new {id = player.Id}, player);
        }
    }
}
