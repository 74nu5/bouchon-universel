namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The reponse bouchonnee.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class ReponseBouchonnee
    {
        /// <summary>Gets or sets the body.</summary>
        public string Body { get; set; }

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
