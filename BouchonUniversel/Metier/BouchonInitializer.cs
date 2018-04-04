namespace BouchonUniversel.Metier
{
    #region Usings

    using DAL;

    using JetBrains.Annotations;

    using Microsoft.Extensions.Options;

    using Models;

    #endregion

    /// <summary>The bouchon initializer.</summary>
    [UsedImplicitly]
    public class BouchonInitializer
    {
        #region Champs

        /// <summary>The bouchon.</summary>
        private readonly IOptions<ApplicationSettings> settings;

        /// <summary>The context.</summary>
        private readonly MemoryContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonInitializer"/> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The bouchon.</param>
        public BouchonInitializer(MemoryContext context, IOptions<ApplicationSettings> settings)
        {
            this.context = context;
            this.settings = settings;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The initialize.</summary>
        public void Initialize() => DbInitializer.Init(this.context, this.settings);

        #endregion
    }
}