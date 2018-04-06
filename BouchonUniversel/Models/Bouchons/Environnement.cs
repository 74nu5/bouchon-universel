namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;

    using Metier;

    #endregion

    /// <summary>The environnemment.</summary>
    public class Environnement : IDto<long>
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the environnement id.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the nom.</summary>
        public string Nom { get; set; }

        /// <summary>Gets or sets the services.</summary>
        public List<Service> Services { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        #endregion
    }
}