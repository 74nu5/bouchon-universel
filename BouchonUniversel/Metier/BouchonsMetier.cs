namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Exceptions;
    using BouchonUniversel.Utils;

    using JetBrains.Annotations;

    using KeyNotFoundException = BouchonUniversel.Exceptions.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon metier.</summary>
    [UsedImplicitly]
    public class BouchonsMetier
    {
        #region Champs et constantes statiques

        /// <summary>The prefix file get.</summary>
        private const string PrefixFileGet = "GET_";

        #endregion

        #region Champs

        /// <summary>The environnement dao.</summary>
        private readonly EnvironnementDAO environnementDAO;

        /// <summary>The services dao.</summary>
        private readonly ServicesDAO servicesDAO;

        /// <summary>The settings bouchon dao.</summary>
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>
        ///     Initializes a new instance of the <see cref="BouchonsMetier" /> class.
        /// </summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        /// <param name="settingsBouchonDAO">The settings Bouchon DAO.</param>
        public BouchonsMetier(ServicesDAO servicesDAO, EnvironnementDAO environnementDAO, SettingsBouchonDAO settingsBouchonDAO)
        {
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
            this.settingsBouchonDAO = settingsBouchonDAO;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The process request.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="string" />.</returns>
        public async Task<string> ProcessGetRequestAsync(string cle, string env, string route, Dictionary<string, string> query)
        {
            var requestIsActivated = this.ServiceIsActivated(cle, env);

            var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), route));
            if (!bouchonDir.Exists)
            {
                bouchonDir.Create();
            }

            var queryStr = string.Join("&", query.Select(pair => $"{pair.Key}={pair.Value}").ToArray());
            var fileName = Path.Combine(bouchonDir.FullName, $"{PrefixFileGet}{queryStr}");

            if (requestIsActivated)
            {
                return File.ReadAllText(fileName);
            }

            var urlBase = new Uri(this.servicesDAO.GetUrl(cle, env));
            var url = new Uri(urlBase, new Uri(route + queryStr));

            var result = await HttpService.GetAsync(url.ToString(), null);

            File.WriteAllText(fileName, result);

            return result;
        }

        /// <summary>The process post request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="body">The body.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="Task" />.</returns>
        public Task<string> ProcessPostRequestAsync(string cle, string env, string route, Dictionary<string, string> query, string body) => throw new NotImplementedException();

        #endregion

        #region Méthodes privées

        /// <summary>The assert service exists.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="bool" />.</returns>
        private bool ServiceIsActivated(string cle, string env)
        {
            if (!this.servicesDAO.ExistsByCle(cle))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            if (!this.environnementDAO.ExistsByName(env))
            {
                throw new EnvironmentNotFoundException("L'environnement n'existe pas");
            }

            return this.servicesDAO.IsActivated(cle) && this.environnementDAO.IsActivated(env);
        }

        #endregion
    }
}