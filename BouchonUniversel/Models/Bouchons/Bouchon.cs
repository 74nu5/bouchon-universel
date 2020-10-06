namespace BouchonUniversel.Models.Bouchons
{
    /// <summary>The bouchon.</summary>
    public class Bouchon : IDto<long>
    {
        /// <summary>Gets or sets the base url.</summary>
        public string BaseUrl { get; set; }

        /// <summary>Gets or sets the id.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets a value indicating whether is enabled.</summary>
        public bool IsEnabled { get; set; }

        /// <summary>Gets or sets the service url.</summary>
        public string ServiceUrl { get; set; }
    }
}
