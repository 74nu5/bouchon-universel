namespace BouchonUniversel.Metier
{
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using DAL.DAO;

    using Exceptions;

    using KeyNotFoundException = Exceptions.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon metier.</summary>
    public class BouchonsMetier
    {
        #region Champs

        /// <summary>The environnement dao.</summary>
        private readonly EnvironnementDAO environnementDAO;

        private readonly SettingsBouchonDAO settingsBouchonDAO;

        /// <summary>The services dao.</summary>
        private readonly ServicesDAO servicesDAO;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier"/> class. Initializes a new instance of the<see cref="T:System.Object"></see> class.</summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
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
        /// <returns>The <see cref="string"/>.</returns>
        public string ProcessRequest(string cle, string env, string route, Dictionary<string, string> query)
        {
            if (!this.servicesDAO.ExistsByCle(cle))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            if (!this.environnementDAO.ExistsByName(env))
            {
                throw new EnvironmentNotFoundException("L'environnement n'existe pas");
            }

            var requestExisted = this.servicesDAO.IsActivated(cle) && this.environnementDAO.IsActivated(env);
            
            if (requestExisted)
            {
                var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), route));
                if (!bouchonDir.Exists)
                {
                    bouchonDir.Create();
                }

                var fileName = string.Join("&", query.Select(pair => $"{pair.Key}={pair.Value}").ToArray());
                return File.ReadAllText(Path.Combine(bouchonDir.FullName, fileName));
            }
            else
            {
                
            }

            return null;
        }

        #endregion
    }
}