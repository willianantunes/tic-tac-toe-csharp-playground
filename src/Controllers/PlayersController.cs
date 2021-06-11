using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using TicTacToeCSharpPlayground.Helper;
using TicTacToeCSharpPlayground.Repository;

namespace TicTacToeCSharpPlayground.Controllers
{
    [Route("tic-tac-toe/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly CSharpPlaygroundContext _context;

        public PlayersController(CSharpPlaygroundContext context, ILogger<PlayersController> logger)
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

            if (player.IsNull())
            {
                Log.Information("No player has been found!");
                return NotFound();
            }
                
            return player;
        }

        [HttpPost]
        public async Task<ActionResult<Player>> CreateNewPlayer(Player player)
        {
            if (player.Name.IsNull())
                return BadRequest("Name is required to create a player");

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecificPlayer", new {id = player.Id}, player);
        }
    }
}
