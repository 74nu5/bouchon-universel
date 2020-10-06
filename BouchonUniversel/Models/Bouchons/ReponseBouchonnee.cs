namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Serialization;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The reponse bouchonnee.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ReponseBouchonnee
    {
        private static readonly XmlDocument XMLDoc = new XmlDocument();

        /// <summary>Gets or sets the body.</summary>
        [XmlIgnore]
        public string Body { get; set; }

        /// <summary>
        ///     Gets or sets the body.
        /// </summary>
        [XmlElement("Body")]
        public XmlCDataSection BodyCData
        {
            get => XMLDoc.CreateCDataSection(this.Body);
            set => this.Body = value?.Data;
        }

        /// <summary>Gets or sets the headers.</summary>
        public List<KeyValue> Headers { get; set; }

        /// <summary>Gets or sets the request.</summary>
        public Request Request { get; set; }

        /// <summary>Gets or sets the response phrase.</summary>
        public string ResponsePhrase { get; set; }

        /// <summary>Gets or sets the status code.</summary>
        public int StatusCode { get; set; }
    }
}
