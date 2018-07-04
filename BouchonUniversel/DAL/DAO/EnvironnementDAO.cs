namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    #region Usings

    using System.Linq;

    using Models.Bouchons;

    #endregion

    #endregion

    /// <summary>The environnement dao.</summary>
    public class EnvironnementDAO : BaseDAO<DataContext, Environnement, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="EnvironnementDAO" /> class.</summary>
        /// <param name="context">The context.</param>
        public EnvironnementDAO(DataContext context) : base(context)
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The exists by name.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool ExistsByName(string name)
            => this.Entities.Any(environnement => environnement.Nom == name);

        #endregion

        #region Méthodes internes

        /// <summary>The is activated.</summary>
        /// <param name="env">The env.</param>
        /// <returns>The <see cref="bool" />.</returns>
        internal bool IsActivated(string env)
            => this.Entities.FirstOrDefault(environnement => environnement.Nom == env)?.IsEnabled ?? false;

        #endregion
    }
}