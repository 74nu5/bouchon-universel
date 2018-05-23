namespace BouchonUniversel.Utils
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    using Models.Bouchons;

    #endregion

    /// <summary>The response utils.</summary>
    internal static class ResponseUtils
    {
        #region Méthodes publiques

        /// <summary>The to key value list.</summary>
        /// <param name="dico">The dico.</param>
        /// <returns>The <see cref="List{KeyValue}" />.</returns>
        public static List<KeyValue> ToKeyValueList(this Dictionary<string, IEnumerable<string>> dico)
            => dico?.Select(kvp => new KeyValue { Key = kvp.Key, Value = kvp.Value.ToArray() }).ToList();

        /// <summary>The to key value list.</summary>
        /// <param name="dico">The dico.</param>
        /// <returns>The <see cref="List{KeyValue}" />.</returns>
        public static List<KeyValue> ToKeyValueList(this Dictionary<string, string[]> dico) => dico?.Select(kvp => new KeyValue { Key = kvp.Key, Value = kvp.Value }).ToList();

        #endregion
    }
}