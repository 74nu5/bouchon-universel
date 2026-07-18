namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.Models.Bouchons;

    using JetBrains.Annotations;

    using Microsoft.EntityFrameworkCore;

    #endregion

    #endregion

    /// <summary>The bouchons dao.</summary>
    [UsedImplicitly]
    public class ServicesDAO : BaseDAO<DataContext, Service, long>
    {
        /// <summary>Initializes a new instance of the <see cref="ServicesDAO" /> class. </summary>
        /// <param name="context">The context.</param>
        public ServicesDAO(DataContext context)
            : base(context)
        {
        }

        /// <summary>The exists by cle.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> ExistsByCleAsync(string cle)
            => await this.Querable.AnyAsync(service => service.Cle == cle);

        /// <summary>The get url.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="environnement">The environnement.</param>
        /// <returns>The <see cref="string" />.</returns>
        public async Task<Uri> GetUrlAsync(string cle, string environnement)
            => new ((await this.Querable.Include(service => service.Environnement).FirstOrDefaultAsync(service => service.Cle == cle && service.Environnement.Nom == environnement))?.Url ?? string.Empty);

        /// <summary>Récupère le service correspondant à une clé et un environnement (avec ses paramètres de chaos).</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="environnement">The environnement.</param>
        /// <returns>The <see cref="Service" /> ou <c>null</c>.</returns>
        public async Task<Service> GetByCleEnvAsync(string cle, string environnement)
            => await this.Querable.Include(service => service.Environnement).FirstOrDefaultAsync(service => service.Cle == cle && service.Environnement.Nom == environnement);

        /// <summary>Update dates is enable.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> IsEnabledToUpdateDatesAsync(string cle)
            => (await this.Querable.FirstOrDefaultAsync(service => service.Cle == cle))?.UpdateDates ?? false;

        /// <summary>The is activated.</summary>
        /// <param name="cle">The cle.</param>
        /// <returns>The <see cref="bool" />.</returns>
        internal async Task<bool> IsActivated(string cle)
            => (await this.Querable.FirstOrDefaultAsync(service => service.Cle == cle))?.IsEnabled ?? false;
    }
}
