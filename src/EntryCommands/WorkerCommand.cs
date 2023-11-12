using System;
using System.Reflection;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TicTacToeCSharpPlayground.Consumers;
using TicTacToeCSharpPlayground.Infrastructure.Database;

namespace TicTacToeCSharpPlayground.EntryCommands
{
    [Command("worker")]
    public class WorkerCommand : ICommand
    {
        [CommandOption("queue-name", IsRequired = true)]
        public string QueueName { get; init; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            Log.Information("Initializing WORKER...");
            await Program.CreateWorkerHostBuilder(new[] {$"QueueName={QueueName}"}).Build().RunAsync();
        }

        public class Startup
        {
            public IConfiguration Configuration { get; }

            public Startup(IConfiguration configuration)
            {
                Configuration = configuration;
            }

            public void ConfigureServices(IServiceCollection services)
            {
                // Database
                var databaseConnectionString = Configuration.GetConnectionString("AppDbContext");
                services.AddDbContext<AppDbContext>(optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(databaseConnectionString);
                });

                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null)
                    throw new InvalidOperationException("Cannot find entry assembly");

                // RabbitMQ
                var brokerConnectionString = Configuration.GetConnectionString("Broker");
                var queueName = Configuration.GetSection("QueueName").Value;
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<PlayerConsumer>();
                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(brokerConnectionString);
                        cfg.ReceiveEndpoint(queueName, e =>
                        {
                            // https://masstransit.io/documentation/configuration/serialization#raw-json
                            e.UseRawJsonDeserializer();
                            e.ConfigureConsumer<PlayerConsumer>(context);
                        });
                        cfg.ConfigureEndpoints(context);
                    });
                });
                // Additional setup
                services.AddHealthChecks()
                    .AddNpgSql(databaseConnectionString, healthQuery: "SELECT 1")
                    .AddRabbitMQ(rabbitConnectionString: brokerConnectionString, name: "rabbitmq");
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app
                    .UseRouting()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapHealthChecks("/healthcheck/liveness", new HealthCheckOptions()
                        {
                            Predicate = _ => false,
                            AllowCachingResponses = false,
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                        endpoints.MapHealthChecks("/healthcheck/readiness", new HealthCheckOptions()
                        {
                            Predicate = _ => true,
                            AllowCachingResponses = false,
                            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                        });
                    });
            }
        }
    }
}
