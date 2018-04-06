namespace BouchonUniversel.Models.Bouchons
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>The bouchon.</summary>
    public class Bouchon
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the base url.</summary>
        [Key]
        public string BaseUrl { get; set; }

        /// <summary>Gets or sets the service url.</summary>
        [Key]
        public string ServiceUrl { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        #endregion
    }
}