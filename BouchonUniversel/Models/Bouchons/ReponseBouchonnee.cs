namespace BouchonUniversel.Models.Bouchons
{
    #region Usings

    using System.Collections.Generic;
    using JetBrains.Annotations;

    #endregion

    [UsedImplicitly]
    public class ReponseBouchonnee
    {
        #region Propriétés et indexeurs

        public Dictionary<string, string> Headers { get; set; }

        public string Body { get; set; }

        public string Query { get; set; }

        public string Route { get; set; }

        #endregion
    }
}