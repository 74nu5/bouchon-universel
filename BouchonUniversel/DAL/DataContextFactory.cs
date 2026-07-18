namespace BouchonUniversel.DAL
{
    #region Usings

    using System.IO;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    #endregion

    /// <summary>
    ///     Fabrique utilisée au moment du design (outils <c>dotnet ef</c>) pour instancier le <see cref="DataContext" />.
    ///     Nécessaire car <c>Program.cs</c> n'expose pas de <c>CreateHostBuilder</c> standard : cette fabrique garantit
    ///     que la génération et l'application des migrations utilisent la même chaîne de connexion que l'application.
    /// </summary>
    public sealed class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        /// <summary>Crée une instance de <see cref="DataContext" /> pour les outils EF Core.</summary>
        /// <param name="args">Arguments transmis par les outils.</param>
        /// <returns>The <see cref="DataContext" />.</returns>
        public DataContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = configuration.GetConnectionString("BDDConnection") };

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(connectionStringBuilder.ToString())
                .Options;

            return new DataContext(options);
        }
    }
}
