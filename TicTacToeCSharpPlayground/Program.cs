using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using TicTacToeCSharpPlayground.Helper;

namespace TicTacToeCSharpPlayground
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nLog = NLogBuilder.ConfigureNLog("nlog.config");
            var logger = nLog.GetCurrentClassLogger();
            try
            {
                logger.I("Initializing application...");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.E(exception, "The application stopped suddenly!");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;
                    builder.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseNLog();
    }
}
