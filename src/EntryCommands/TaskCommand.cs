using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TicTacToeCSharpPlayground.Core.Models;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.EntryCommands
{
    [Command("task")]
    public class TaskCommand : ICommand
    {
        [CommandOption("seed", IsRequired = false, Description = "Fill database with fake data")]
        public bool SeedDatabase { get; init; } = false;

        [CommandOption("amount-of-players", IsRequired = false, Description = "Amount of players to be created")]
        public int AmountOfPlayers { get; init; } = 1000;

        [CommandOption("connection-string", IsRequired = true, EnvironmentVariable = "ConnectionStrings__AppDbContext")]
        public string ConnectionString { get; init; }

        [CommandOption("migrate", IsRequired = false, Description = "Apply migrations")]
        public bool Migrate { get; init; } = false;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (SeedDatabase is true || Migrate is true)
            {
                var dbContext = AppDbContext.CreateContext(ConnectionString);
                if (Migrate)
                {
                    if (dbContext.Database.GetPendingMigrations().ToList().Any() is true)
                    {
                        await dbContext.Database.MigrateAsync();
                        await console.Output.WriteLineAsync("Migrate executed!");
                    }
                }
                if (SeedDatabase)
                {
                    await CreateScenarioWithGivenAmountOfPlayers(dbContext, AmountOfPlayers);
                    await console.Output.WriteLineAsync("Seed has been executed!");
                }
                await console.Output.WriteLineAsync("Done 🤙");
            }
            else
            {
                await console.Output.WriteLineAsync("Nothing to do 🤷");
            }
        }

        private async Task CreateScenarioWithGivenAmountOfPlayers(AppDbContext dbContext, int amount)
        {
            var areThereAnyPlayers = await dbContext.Players.AnyAsync();
            if (areThereAnyPlayers is not true)
            {
                var playersToBeSaved = new List<Player>();

                foreach (int index in Enumerable.Range(1, amount))
                {
                    var isComputer = index % 2 == 0;
                    var player = new Player { Name = $"Player {index}", Computer = isComputer };
                    playersToBeSaved.Add(player);
                }

                dbContext.AddRange(playersToBeSaved);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
