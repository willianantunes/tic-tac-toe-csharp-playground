using System.IO;
using System.Threading.Tasks;
using CliFx;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using TicTacToeCSharpPlayground.EntryCommands;

namespace TicTacToeCSharpPlayground
{
    public class Program
    {
        internal static IConfiguration Configuration { get; set; }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();
        }

        private static async Task<int> InitiateCommandLineInterfaceProgram()
        {
            // Know more at: https://github.com/Tyrrrz/CliFx
            return await new CliApplicationBuilder()
                .SetExecutableName("TicTacToeCSharpPlayground")
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
        }

        private static async Task<int> Main(string[] args)
        {
            Configuration = BuildConfiguration();

            ConfigureLogger();

            return await InitiateCommandLineInterfaceProgram();
        }

        public static IConfigurationRoot BuildConfiguration(IConfigurationBuilder? configuration = null)
        {
            var solutionSettings = Path.Combine(Directory.GetCurrentDirectory(), "../", "appsettings.json");
            if (File.Exists(solutionSettings) is false)
                solutionSettings = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            configuration ??= new ConfigurationBuilder();
            
            return configuration
                .AddJsonFile(solutionSettings, optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // It's here because of EF: https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#from-application-services
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, configurationBuilder) => BuildConfiguration(configurationBuilder))
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<ApiCommand.Startup>(); });
        
        public static IHostBuilder CreateWorkerHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configurationBuilder) => BuildConfiguration(configurationBuilder))
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
            .ConfigureWebHostDefaults(builder => { builder.UseStartup<WorkerCommand.Startup>(); });
    }
}
