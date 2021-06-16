using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly DbSet<Player> _databaseSet;

        public PlayersController(AppDbContext context)
        {
            _context = context;
            _databaseSet = context.Players;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetAllPlayers()
        {
            Log.Information("Getting all players...");
            return await _databaseSet.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetSpecificPlayer(int id)
        {
            Log.Information("Getting specific player given ID: {Id}", id);
            var player = await _databaseSet.FindAsync(id);

            if (player is null)
            {
                Log.Information("No player has been found");
                return NotFound();
            }

            return player;
        }

        [HttpPost]
        public async Task<ActionResult<Player>> CreateNewPlayer(Player player)
        {
            await _databaseSet.AddAsync(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecificPlayer", new { id = player.Id }, player);
        }
    }
}
