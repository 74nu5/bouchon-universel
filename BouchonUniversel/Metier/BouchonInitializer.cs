namespace BouchonUniversel.Metier
{
    #region Usings

    using BouchonUniversel.DAL;
    using BouchonUniversel.Models;

    using JetBrains.Annotations;

    using Microsoft.Extensions.Options;

    #endregion

    /// <summary>The bouchon initializer.</summary>
    [UsedImplicitly]
    public class BouchonInitializer
    {
        /// <summary>The context.</summary>
        private readonly DataContext context;

        /// <summary>The bouchon.</summary>
        private readonly IOptions<ApplicationSettings> settings;

        /// <summary>Initializes a new instance of the <see cref="BouchonInitializer" /> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The bouchon.</param>
        public BouchonInitializer(DataContext context, IOptions<ApplicationSettings> settings)
        {
            this.context = context;
            this.settings = settings;
        }

        /// <summary>The initialize.</summary>
        public void Initialize()
            => DbInitializer.Init(this.context, this.settings);
    }
}
