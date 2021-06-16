using System;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using TicTacToeCSharpPlayground.Api.Configs;
using TicTacToeCSharpPlayground.Core.Business;
using TicTacToeCSharpPlayground.Core.Repository;
using TicTacToeCSharpPlayground.Core.Services;
using TicTacToeCSharpPlayground.Infrastructure.Database;
using TicTacToeCSharpPlayground.Infrastructure.Database.Repositories;

namespace TicTacToeCSharpPlayground.EntryCommands
{
    [Command("api")]
    public class ApiCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            // We create our host and run our web api!
            await Program.CreateHostBuilder(Array.Empty<string>()).Build().RunAsync();
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
                // APIs
                services.AddControllers(options => { options.Filters.Add(new HttpExceptionFilter()); });
                services.AddControllers(options =>
                {
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                });
                services.AddSwaggerGen(swaggerGenOptions =>
                {
                    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "TicTacToeCSharpPlayground", Version = "v1" });
                });
                // Database
                services.AddHttpContextAccessor().AddDbContext<AppDbContext>(optionsBuilder =>
                {
                    var connectionString = Configuration.GetConnectionString("AppDbContext");
                    optionsBuilder.UseNpgsql(connectionString);
                });
                // Helpers
                // https://docs.automapper.org/en/latest/Dependency-injection.html#asp-net-core
                services.AddAutoMapper(typeof(Startup));
                // Repositories
                services.AddScoped<ITicTacToeRepository, TicTacToeRepository>();
                // Services
                services.AddScoped<IGameService, GameService>();
                // Businesses
                services.AddSingleton<IBoardJudge, BoardJudge>();
                services.AddSingleton<IPositionDecider, PositionDecider>();
                services.AddScoped<IBoardDealer, BoardDealer>();
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TicTacToeCSharpPlayground v1"));
                }

                app.UseSerilogRequestLogging();
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            }
        }
    }
}
