using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Infrastructure;
using FluentAssertions;
using Tests.Support;
using TicTacToeCSharpPlayground.EntryCommands;
using TicTacToeCSharpPlayground.Infrastructure.Database;
using Xunit;

namespace Tests.TicTacToeCSharpPlayground.EntryCommands
{
    public class TaskCommandITests
    {
        [Fact]
        public async Task ShouldDoNothingGivenNoParametersAreProvided()
        {
            // Arrange
            using var console = new FakeInMemoryConsole();
            var app = new CliApplicationBuilder()
                .AddCommand<TaskCommand>()
                .UseConsole(console)
                .Build();
            var invokingTaskWithoutArguments = new[] {"task"};
            var emptyEnvVars = new Dictionary<string, string>();
            // Act
            await app.RunAsync(invokingTaskWithoutArguments, emptyEnvVars);
            // Assert
            var stdOut = console.ReadOutputString();
            var stdOutError = console.ReadErrorString();
            stdOut.Should().Be("Nothing to do 🤷\n");
            stdOutError.Should().BeEmpty();
        }

        [Fact]
        public async Task ShouldInformErrorGivenConnectionStringIsNotAvailable()
        {
            // Arrange
            using var console = new FakeInMemoryConsole();
            var app = new CliApplicationBuilder()
                .AddCommand<TaskCommand>()
                .UseConsole(console)
                .Build();
            var invokingTaskWithSeed = new[] {"task", "--seed"};
            var emptyEnvVars = new Dictionary<string, string>();
            // Act
            await app.RunAsync(invokingTaskWithSeed, emptyEnvVars);
            // Assert
            var stdOut = console.ReadOutputString();
            var stdOutError = console.ReadErrorString();
            stdOut.Should().BeEmpty();
            stdOutError.Should().Be("You should add connection string using --connection-string option\n");
        }

        public class WithDatabase : IntegrationTestsWithDependencyInjection
        {
            [Fact]
            public async Task ShouldSeedDatabase()
            {
                // Arrange
                using var console = new FakeInMemoryConsole();
                var app = new CliApplicationBuilder()
                    .AddCommand<TaskCommand>()
                    .UseConsole(console)
                    .Build();
                var invokingTaskWithSeed = new[] {"task", "--seed"};
                var envKey = "ConnectionStrings__AppDbContext";
                var connectionString = ConnectionString;
                var envVars = new Dictionary<string, string>
                {
                    {
                        envKey,
                        connectionString
                    }
                };
                // Act
                await app.RunAsync(invokingTaskWithSeed, envVars);
                // Assert
                AppDbContext.CreateContext(ConnectionString).Players.Count().Should().Be(1000);
                var stdOut = console.ReadOutputString();
                var stdOutError = console.ReadErrorString();
                stdOut.Should().Be("Seed has been executed!\n");
                stdOutError.Should().BeEmpty();
            }

            [Fact]
            public async Task ShouldSeedDatabaseWithCustomAmountOfPlayers()
            {
                // Arrange
                using var console = new FakeInMemoryConsole();
                var app = new CliApplicationBuilder()
                    .AddCommand<TaskCommand>()
                    .UseConsole(console)
                    .Build();
                var amountOfPlayersToBeCreated = 10_000;
                var invokingTaskWithSeed = new[]
                    {"task", "--seed", "--amount-of-players", amountOfPlayersToBeCreated.ToString()};
                var envKey = "ConnectionStrings__AppDbContext";
                var connectionString = ConnectionString;
                var envVars = new Dictionary<string, string>
                {
                    {
                        envKey,
                        connectionString
                    }
                };
                // Act
                await app.RunAsync(invokingTaskWithSeed, envVars);
                // Assert
                AppDbContext.CreateContext(ConnectionString).Players.Count().Should().Be(amountOfPlayersToBeCreated);
                var stdOut = console.ReadOutputString();
                var stdOutError = console.ReadErrorString();
                stdOut.Should().Be("Seed has been executed!\n");
                stdOutError.Should().BeEmpty();
            }
        }
    }
}
