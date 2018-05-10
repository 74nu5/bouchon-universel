namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The request.</summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public sealed class Request
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the body.</summary>
        public string Body { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        public List<KeyValue> Headers { get; set; }

        /// <summary>Gets or sets the query.</summary>
        public List<KeyValue> Query { get; set; }

        /// <summary>Gets or sets the route.</summary>
        public string Route { get; set; }

        #endregion
    }
}