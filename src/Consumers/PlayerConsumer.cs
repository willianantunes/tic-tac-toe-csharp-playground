using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.Consumers;

public class PlayerConsumer : IConsumer<Player>
{
    private readonly ILogger<PlayerConsumer> _logger;
    private readonly AppDbContext _context;

    public PlayerConsumer(ILogger<PlayerConsumer> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task Consume(ConsumeContext<Player> context)
    {
        _context.Players.Add(context.Message);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Player {Id} has been created", context.Message.Id);
    }
}
