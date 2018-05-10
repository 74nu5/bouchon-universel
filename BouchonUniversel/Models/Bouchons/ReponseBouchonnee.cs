namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The reponse bouchonnee.</summary>
    [UsedImplicitly]
    public class ReponseBouchonnee
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the body.</summary>
        public string Body { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>Gets or sets the query.</summary>
        public string Query { get; set; }

        /// <summary>Gets or sets the route.</summary>
        public string Route { get; set; }

        #endregion
    }
}