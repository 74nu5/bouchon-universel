namespace BouchonUniversel.Models.Bouchons
{
    /// <summary>The services.</summary>
    public class Service : IDto<long>
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the cle.</summary>
        public string Cle { get; set; }

        /// <summary>Gets or sets the environnement id.</summary>
        public long EnvironnementId { get; set; }

        /// <summary>Gets or sets the environnement.</summary>
        public Environnement Environnement { get; set; }

        /// <summary>Gets or sets the url.</summary>
        public string Url { get; set; }

        /// <summary>Gets or sets the id.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        #endregion
    }
}