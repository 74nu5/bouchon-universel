namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using JetBrains.Annotations;

    #endregion

    /// <summary>The key value.</summary>
    [UsedImplicitly]
    public class KeyValue
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the key.</summary>
        public string Key { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public string Value { get; set; }

        #endregion
    }
}