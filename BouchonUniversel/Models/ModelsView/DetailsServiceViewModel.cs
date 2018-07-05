namespace BouchonUniversel.Models.ModelsView
{
    #region Usings

    using BouchonUniversel.Models.Bouchons;

    #endregion

    /// <summary>The details service view model.</summary>
    public sealed class DetailsServiceViewModel
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the liste fichiers.</summary>
        public DirectoryBouchon ListeFichiers { get; set; }

        /// <summary>Gets or sets the service.</summary>
        public Service Service { get; set; }

        #endregion
    }
}