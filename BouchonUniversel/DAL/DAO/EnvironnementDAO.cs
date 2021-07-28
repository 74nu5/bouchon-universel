namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System.Threading.Tasks;

    using BouchonUniversel.Models.Bouchons;

    using Microsoft.EntityFrameworkCore;

    #endregion

    #endregion

    /// <summary>The environnement dao.</summary>
    public class EnvironnementDAO : BaseDAO<DataContext, Environnement, long>
    {
        /// <summary>Initializes a new instance of the <see cref="EnvironnementDAO" /> class.</summary>
        /// <param name="context">The context.</param>
        public EnvironnementDAO(DataContext context)
            : base(context)
        {
        }

        /// <summary>The exists by name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> ExistsByNameAsync(string name)
            => await this.Querable.AnyAsync(environnement => environnement.Nom == name);

        /// <summary>The is activated.</summary>
        /// <param name="env">The env.</param>
        /// <returns>The <see cref="bool" />.</returns>
        internal async Task<bool> IsActivatedAsync(string env)
            => (await this.Querable.FirstOrDefaultAsync(environnement => environnement.Nom == env))?.IsEnabled ?? false;
    }
}
