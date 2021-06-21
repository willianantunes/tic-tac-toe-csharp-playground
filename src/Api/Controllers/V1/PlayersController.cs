using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DrfLikePaginations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TicTacToeCSharpPlayground.Core.DTOSetup;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.Api.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPagination _pagination;
        private readonly IMapper _mapper;

        public PlayersController(AppDbContext context, IPagination pagination, IMapper mapper)
        {
            _context = context;
            _pagination = pagination;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<Paginated<PlayerDTO>>> GetAllPlayers()
        {
            Log.Information("Getting all players...");
            var query = _context.Players.AsNoTracking().OrderBy(p => p.CreatedAt);
            var displayUrl = Request.GetDisplayUrl();
            var queryParams = Request.Query;
            Func<Player, PlayerDTO> transform = p => _mapper.Map<Player, PlayerDTO>(p);

            return await _pagination.CreateAsync(query, displayUrl, queryParams, transform);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetSpecificPlayer(int id)
        {
            Log.Information("Getting specific player given ID: {Id}", id);
            var player = await _context.Players.FindAsync(id);

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
            _context.Players.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecificPlayer", new { id = player.Id }, player);
        }

        [HttpPut]
        public async Task<IActionResult> PutTodoItem(Player player)
        {
            _context.Entry(player).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_context.Players.Any(p => p.Id == player.Id) is false)
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Player>> DeletePlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player is null)
            {
                Log.Information("No player has been found");
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return player;
        }
    }
}
