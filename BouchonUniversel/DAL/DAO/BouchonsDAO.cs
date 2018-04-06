namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    using Models.Bouchons;

    #endregion

    /// <summary>The bouchons dao.</summary>
    public class BouchonsDAO : BaseDAO<DataContext, Bouchon, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsDAO"/> class. </summary>
        /// <param name="context">The context.</param>
        public BouchonsDAO(DataContext context) : base(context)
        {
        }

        #endregion
    }
}