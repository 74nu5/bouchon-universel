namespace BouchonUniversel
{
    #region Usings

    using System;

    using BouchonUniversel.Metier;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    #endregion

    /// <summary>The program.</summary>
    [UsedImplicitly]
    public class Program
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            // InitializeData(host);
            host.Run();
        }

        /// <summary>The build web host.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="IWebHost" />.</returns>
        private static IHost BuildWebHost(string[] args) => Host.CreateDefaultBuilder(args)
                                                                .ConfigureWebHostDefaults(
                                                                                          builder => builder.UseStartup<Startup>().UseUrls("https://*:5555"))
                                                                .Build();

        /// <summary>The initialize data.</summary>
        /// <param name="host">The host.</param>
        private static void InitializeData(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                services.GetRequiredService<BouchonInitializer>().Initialize();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
