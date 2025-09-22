using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GoodsEnterprise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Logging

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var logLevel = config.GetSection("Logging")?.GetSection("Default")?.Value;
            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithCorrelationIdHeader("CorrelationId")
                .WriteTo.Console(new CompactJsonFormatter())
                .WriteTo.File(path: Path.Combine(AppContext.BaseDirectory, "Logs", "log.txt"),
                rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, retainedFileCountLimit: 31, shared: true, buffered: false);
            //.WriteTo.RollingFileAlternate(new CompactJsonFormatter(), ".");
            switch (logLevel)
            {
                case "Error":
                    logConfig.MinimumLevel.Error();
                    break;
                case "Information":
                    logConfig.MinimumLevel.Information();
                    break;
                default:
                    logConfig.MinimumLevel.Debug();
                    break;
            }
            Log.Logger = logConfig.CreateLogger();

            #endregion
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

