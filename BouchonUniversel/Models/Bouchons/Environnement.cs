namespace BouchonUniversel.Models.Bouchons
{
    using System.Collections.Generic;

    /// <summary>The environnemment.</summary>
    public class Environnement
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the environnement id.</summary>
        public long EnvironnementId { get; set; }

        /// <summary>Gets or sets the nom.</summary>
        public string Nom { get; set; }

        public List<Service> Services { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        #endregion
    }
}