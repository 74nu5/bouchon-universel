namespace BouchonUniversel.DAL
{
    #region Usings

    using System.Linq;

    using Microsoft.Extensions.Options;

    using Models;

    #endregion

    /// <summary>The db initializer.</summary>
    internal static class DbInitializer
    {
        #region Méthodes publiques

        /// <summary>The init.</summary>
        /// <param name="context">The context.</param>
        /// <param name="options">The options.</param>
        public static void Init(DataContext context, IOptions<ApplicationSettings> options)
        {
            context.Database.EnsureCreated();

            if (context.SettingsBouchon.Any())
            {
                return;
            }

            var defautActivationSettings = new SettingsBouchon
            {
                Key = "IsActivated",
                Value = options.Value.DefautActivation.ToString()
            };

            var cheminFichiers = new SettingsBouchon
            {
                Key = nameof(options.Value.CheminFichiers),
                Value = options.Value.CheminFichiers
            };

            var urlService = new SettingsBouchon
            {
                Key = nameof(options.Value.UrlService),
                Value = options.Value.UrlService
            };

            context.Add(defautActivationSettings);
            context.Add(cheminFichiers);
            context.Add(urlService);

            context.SaveChanges(true);
        }

        #endregion
    }
}