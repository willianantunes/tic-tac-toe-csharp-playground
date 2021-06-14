using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using TicTacToeCSharpPlayground;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace Tests.Support
{
    public abstract class IntegrationTestsFixture<TStartup> : IDisposable where TStartup : class
    {
        protected readonly HttpClient Client;
        protected readonly AppDbContext AppDbContext;
        protected readonly IServiceProvider Services;
        private readonly IDbContextTransaction _transaction;

        protected IntegrationTestsFixture(Action<IServiceCollection> customSetup = null)
        {
            // Basic setup
            var configuration = Program.BuildConfiguration();
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<TStartup>()
                .UseSerilog();
            // You can create new clients with mocked services if required (see ConfigureTestServices)
            var dbName = $"test_{Guid.NewGuid()}";
            // https://github.com/dotnet/aspnetcore/issues/12079
            builder.UseEnvironment("Production");
            builder.ConfigureTestServices(ConfigureDatabaseAsSingleton(configuration, dbName));
            if (customSetup is not null)
                builder.ConfigureTestServices(customSetup);
            var server = new TestServer(builder);
            Services = server.Host.Services;
            // It allows you to call API endpoints
            Client = server.CreateClient();
            // It allows you to consult the database
            AppDbContext = Services.GetRequiredService<AppDbContext>();
            Trace.Assert(AppDbContext.Database.GetDbConnection().Database == dbName);
            ExecuteMissingMigrations();
            // The transaction will be created by each test method, if you inherit it without an IClassFixture
            _transaction = AppDbContext.Database.BeginTransaction();
        }

        private void ExecuteMissingMigrations()
        {
            AppDbContext.Database.EnsureCreated();
        }

        private Action<IServiceCollection> ConfigureDatabaseAsSingleton(IConfiguration configuration, string dbName)
        {
            var oldDbValue = "Database=postgres;";
            var newDbValue = $"Database={dbName};";

            return services =>
            {
                services.RemoveAll<AppDbContext>();
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.AddDbContext<AppDbContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("AppDbContext");
                    var newConnectionString = connectionString.Replace(oldDbValue, newDbValue);
                    options.UseNpgsql(newConnectionString);
                }, ServiceLifetime.Singleton);
            };
        }

        public void Dispose()
        {
            if (_transaction is not null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
            AppDbContext.Database.EnsureDeleted();
        }
    }
}
