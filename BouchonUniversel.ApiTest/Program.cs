namespace BouchonUniversel.ApiTest
{
    #region Usings

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    #endregion

    /// <summary>The program.</summary>
    internal static class Program
    {
        #region Méthodes publiques

        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args) => BuildWebHost(args).Run();

        #endregion

        #region Méthodes privées

        /// <summary>The build web host.</summary>
        /// <param name="args">The args.</param>
        /// <returns>The <see cref="IWebHost"/>.</returns>
        private static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();

        #endregion
    }
}