using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CodeRedCryptoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args, !Environment.UserInteractive);
        }

        public static void BuildWebHost(string[] args, bool isService)
        {
            var pathToContentRoot = Directory.GetCurrentDirectory();
            pathToContentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var config = new ConfigurationBuilder()
                      .SetBasePath(pathToContentRoot)
                      .AddJsonFile("hosting.json", optional: false, reloadOnChange: true)
                      .Build();

            var host = WebHost.CreateDefaultBuilder(args)
                 .UseContentRoot(pathToContentRoot)
                 .UseConfiguration(config)
                 .UseStartup<Startup>()
                 .UseSerilog((hostingContext, loggerConfiguration) => {
                     loggerConfiguration
                     .ReadFrom.Configuration(hostingContext.Configuration)
                     .Enrich.FromLogContext();
                 })
                 .Build();

                host.Run();
            
        }
    }
}
