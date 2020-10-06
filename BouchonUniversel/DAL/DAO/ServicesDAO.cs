namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System;
    using System.Linq;

    using BouchonUniversel.Models.Bouchons;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    #endregion

    #endregion

    /// <summary>The bouchons dao.</summary>
    [UsedImplicitly]
    public class ServicesDAO : BaseDAO<DataContext, Service, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="ServicesDAO"/> class. </summary>
        /// <param name="context">The context.</param>
        public ServicesDAO(DataContext context)
            : base(context)
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The exists by cle.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ExistsByCle(string cle)
            => this.Querable.Any(service => service.Cle == cle);

        /// <summary>The get url.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="environnement">The environnement.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public Uri GetUrl(string cle, string environnement)
            => new Uri(this.Querable.Include(service => service.Environnement).FirstOrDefault(service => service.Cle == cle && service.Environnement.Nom == environnement)?.Url);

        /// <summary>Update dates is enable.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsEnabledToUpdateDates(string cle)
            => this.Querable.FirstOrDefault(service => service.Cle == cle)?.UpdateDates ?? false;

        #endregion

        #region Méthodes internes

        /// <summary>The is activated.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal bool IsActivated(string cle)
            => this.Querable.FirstOrDefault(service => service.Cle == cle)?.IsEnabled ?? false;

        #endregion
    }
}