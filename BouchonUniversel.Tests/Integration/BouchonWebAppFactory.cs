namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;

    using BouchonUniversel;
    using BouchonUniversel.DAL;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Models.Bouchons;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    #endregion

    /// <summary>
    ///     Fabrique de l'application pour les tests d'intégration bout-en-bout.
    ///     Isole la base SQLite et le répertoire des bouchons dans un dossier temporaire, applique les migrations,
    ///     amorce la configuration puis un service de démonstration (chaos) prêt à l'emploi.
    /// </summary>
    public sealed class BouchonWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string workDirectory;
        private readonly string databasePath;
        private readonly string filesPath;
        private readonly string adminUsername;
        private readonly string adminPasswordHash;
        private readonly string viewerUsername;
        private readonly string viewerPasswordHash;
        private readonly bool seedData;

        /// <summary>Initializes a new instance of the <see cref="BouchonWebAppFactory" /> class.</summary>
        /// <param name="adminUsername">Identifiant admin (active l'authentification si renseigné avec le hash).</param>
        /// <param name="adminPasswordHash">Hash du mot de passe admin.</param>
        /// <param name="seedData">Amorce les paramètres et un service de démonstration ; sinon, seul le schéma est créé (application « non installée »).</param>
        /// <param name="viewerUsername">Identifiant du compte lecteur (facultatif).</param>
        /// <param name="viewerPasswordHash">Hash du mot de passe lecteur (facultatif).</param>
        public BouchonWebAppFactory(string adminUsername = null, string adminPasswordHash = null, bool seedData = true, string viewerUsername = null, string viewerPasswordHash = null)
        {
            this.workDirectory = Path.Combine(Path.GetTempPath(), "bouchon-it-" + Guid.NewGuid().ToString("N"));
            this.databasePath = Path.Combine(this.workDirectory, "it.db");
            this.filesPath = Path.Combine(this.workDirectory, "files");
            Directory.CreateDirectory(this.filesPath);
            this.adminUsername = adminUsername;
            this.adminPasswordHash = adminPasswordHash;
            this.viewerUsername = viewerUsername;
            this.viewerPasswordHash = viewerPasswordHash;
            this.seedData = seedData;
        }

        /// <summary>Gets le répertoire racine des fichiers de bouchons utilisé par cette instance.</summary>
        public string FilesPath => this.filesPath;

        /// <summary>Crée un client en HTTPS (évite le 307 de UseHttpsRedirection sur le serveur de test).</summary>
        /// <param name="allowAutoRedirect">Suivre automatiquement les redirections.</param>
        /// <returns>The <see cref="HttpClient" />.</returns>
        public HttpClient CreateHttpsClient(bool allowAutoRedirect = true)
            => this.CreateClient(new WebApplicationFactoryClientOptions
                                 {
                                     BaseAddress = new Uri("https://localhost"),
                                     AllowAutoRedirect = allowAutoRedirect,
                                 });

        /// <inheritdoc />
        protected override void ConfigureWebHost(IWebHostBuilder builder)
            => builder.ConfigureAppConfiguration((_, config) =>
            {
                var settings = new Dictionary<string, string>
                               {
                                   ["ConnectionStrings:BDDConnection"] = this.databasePath,
                                   ["Bouchon:CheminFichiers"] = this.filesPath,
                                   ["Bouchon:DefautActivation"] = "true",
                                   ["Bouchon:UrlService"] = string.Empty,
                               };

                if (!string.IsNullOrEmpty(this.adminUsername))
                {
                    settings["Admin:Username"] = this.adminUsername;
                    settings["Admin:PasswordHash"] = this.adminPasswordHash;
                }

                if (!string.IsNullOrEmpty(this.viewerUsername))
                {
                    settings["Admin:ViewerUsername"] = this.viewerUsername;
                    settings["Admin:ViewerPasswordHash"] = this.viewerPasswordHash;
                }

                config.AddInMemoryCollection(settings);
            });

        /// <inheritdoc />
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                if (this.seedData)
                {
                    // Applique les migrations et amorce les paramètres depuis la configuration (comme au démarrage réel).
                    provider.GetRequiredService<BouchonInitializer>().Initialize();
                    SeedChaosService(provider.GetRequiredService<DataContext>());
                }
                else
                {
                    // Schéma seulement : l'application est « non installée » (aucun paramètre).
                    provider.GetRequiredService<DataContext>().Database.Migrate();
                }
            }

            return host;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                TryDeleteWorkDirectory(this.workDirectory);
            }
        }

        private static void SeedChaosService(DataContext context)
        {
            if (context.Services.Any(service => service.Cle == "chaos"))
            {
                return;
            }

            var environnement = new Environnement { Nom = "dev", IsEnabled = true };
            context.Environnement.Add(environnement);
            context.Services.Add(new Service
                                 {
                                     Cle = "chaos",
                                     Environnement = environnement,
                                     Url = "http://downstream.invalid/",
                                     IsEnabled = true,
                                     ErrorProbability = 100,
                                     ErrorStatusCode = 503,
                                 });
            context.SaveChanges();
        }

        private static void TryDeleteWorkDirectory(string directory)
        {
            try
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }
            }
            catch (IOException)
            {
                // Nettoyage best-effort : la base peut encore être verrouillée.
            }
        }
    }
}
