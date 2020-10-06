namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System.Linq;

    using BouchonUniversel.Models.Bouchons;

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
        public bool ExistsByName(string name)
            => this.Querable.Any(environnement => environnement.Nom == name);

        /// <summary>The is activated.</summary>
        /// <param name="env">The env.</param>
        /// <returns>The <see cref="bool" />.</returns>
        internal bool IsActivated(string env)
            => this.Querable.FirstOrDefault(environnement => environnement.Nom == env)?.IsEnabled ?? false;
    }
}
