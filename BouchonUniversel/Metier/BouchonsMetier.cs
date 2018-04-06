namespace BouchonUniversel.Metier
{
    #region Usings

    using System.Collections.Generic;
    using System.Net.Http;

    using DAL;

    #endregion

    /// <summary>The bouchon metier.</summary>
    public class BouchonsMetier
    {
        private readonly ServicesDAO servicesDAO;

        #region Champs

        /// <summary>The dao.</summary>
        private readonly BouchonsDAO bouchonsDAO;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier"/> class. Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        /// <param name="dao">The dao.</param>
        public BouchonsMetier(BouchonsDAO bouchonsDAO, ServicesDAO servicesDAO)
        {
            this.servicesDAO = servicesDAO;
            this.bouchonsDAO = this.bouchonsDAO;
        }

        #endregion

        public void ProcessRequest(string cle, string env, string route, Dictionary<string, string> query)
        {
            if (!this.servicesDAO.Exists(cle))
            {
                throw new HttpRequestException("La clé n'existe pas");
            }

            //this.dao.
        }
    }
}