namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DAL.DAO;

    using Exceptions;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

    using Utils.Http;

    using KeyNotFoundException = Exceptions.KeyNotFoundException;

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

        /// <summary>The http.</summary>
        private readonly HttpService http;

        /// <summary>The services dao.</summary>
        private readonly ServicesDAO servicesDAO;

        /// <summary>The settings bouchon dao.</summary>
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier"/> class.</summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        /// <param name="settingsBouchonDAO">The settings Bouchon DAO.</param>
        /// <param name="http">The http.</param>
        public BouchonsMetier(ServicesDAO servicesDAO, EnvironnementDAO environnementDAO, SettingsBouchonDAO settingsBouchonDAO, HttpService http)
        {
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
            this.settingsBouchonDAO = settingsBouchonDAO;
            this.http = http;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The process request.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="string"/>.</returns>
        public async Task<string> ProcessGetRequestAsync(string cle, string env, string route, Dictionary<string, string> query, Dictionary<string, string[]> headers)
            => await this.ProcessRequestAsync(HttpMethod.Get, cle, env, route, query, headers, null);

        /// <summary>The process post request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<string> ProcessPostRequestAsync(string cle, string env, string route, Dictionary<string, string> query, Dictionary<string, string[]> headers, string body)
            => await this.ProcessRequestAsync(HttpMethod.Post, cle, env, route, query, headers, body);

        #endregion

        #region Méthodes privées

        /// <summary>The process request async.</summary>
        /// <param name="method">The get.</param>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<string> ProcessRequestAsync(
            HttpMethod method,
            string cle,
            string env,
            string route,
            Dictionary<string, string> query,
            Dictionary<string, string[]> headers,
            string body)
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

            var result = string.Empty;

            switch (method)
            {
                case HttpMethod.Get:
                    result = await this.http.GetAsync(url.ToString(), headers, null);
                    break;
                case HttpMethod.Put:
                    break;
                case HttpMethod.Delete:
                    break;
                case HttpMethod.Post:
                    result = await this.http.PostAsync(url.ToString(), headers, body, null);
                    break;
                case HttpMethod.Head:
                    break;
                case HttpMethod.Trace:
                    break;
                case HttpMethod.Patch:
                    break;
                case HttpMethod.Connect:
                    break;
                case HttpMethod.Options:
                    break;
                case HttpMethod.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }

            File.WriteAllText(fileName, result);

            return result;
        }

        /// <summary>The assert service exists.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="bool"/>.</returns>
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