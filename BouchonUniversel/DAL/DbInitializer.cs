namespace BouchonUniversel.DAL
{
    #region Usings

    using System.Linq;

    using BouchonUniversel.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    #endregion

    /// <summary>The db initializer.</summary>
    internal static class DbInitializer
    {
        /// <summary>The init.</summary>
        /// <param name="context">The context.</param>
        /// <param name="options">The options.</param>
        public static void Init(DataContext context, IOptions<ApplicationSettings> options)
        {
            // Applique les migrations en attente (crée le schéma et la table d'historique __EFMigrationsHistory).
            // NB : ne jamais appeler EnsureCreated() avant Migrate() — cela court-circuiterait les migrations.
            context.Database.Migrate();

            if (context.SettingsBouchon.Any())
            {
                return;
            }

            var defautActivationSettings = new SettingsBouchon { Key = "IsActivated", Value = options.Value.DefautActivation.ToString() };

            var cheminFichiers = new SettingsBouchon { Key = nameof(options.Value.CheminFichiers), Value = options.Value.CheminFichiers };

            var urlService = new SettingsBouchon { Key = nameof(options.Value.UrlService), Value = options.Value.UrlService };

            context.Add(defautActivationSettings);
            context.Add(cheminFichiers);
            context.Add(urlService);

            context.SaveChanges(true);
        }
    }
}
