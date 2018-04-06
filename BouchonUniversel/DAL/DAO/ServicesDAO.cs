namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System.Linq;

    using Models.Bouchons;

    #endregion

    #endregion

    /// <summary>The bouchons dao.</summary>
    public class ServicesDAO : BaseDAO<DataContext, Service, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="ServicesDAO"/> class. </summary>
        /// <param name="context">The context.</param>
        public ServicesDAO(DataContext context) : base(context)
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The exists by cle.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ExistsByCle(string cle) => this.Entities.Any(service => service.Cle == cle);

        #endregion

        #region Méthodes internes

        /// <summary>The is activated.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal bool IsActivated(string cle) => this.Entities.FirstOrDefault(service => service.Cle == cle)?.IsEnabled ?? false;

        #endregion
    }
}