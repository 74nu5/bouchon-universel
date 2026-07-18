namespace BouchonUniversel.ApiTest
{
    #region Usings

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    #endregion

    /// <summary>The program.</summary>
    internal static class Program
    {
        #region Méthodes publiques

        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args) => BuildHost(args).Run();

        #endregion

        #region Méthodes privées

        /// <summary>The build host.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="IHost"/>.</returns>
        private static IHost BuildHost(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
            .Build();

        #endregion
    }
}