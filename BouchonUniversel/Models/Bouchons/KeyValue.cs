namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Xml.Serialization;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The key value.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class KeyValue
    {
        /// <summary>Gets or sets the key.</summary>
        [XmlAttribute]
        public string Key { get; set; }

        /// <summary>Gets or sets the value.</summary>
        [XmlAttribute]
        public string[] Value { get; set; }
    }
}
