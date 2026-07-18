namespace BouchonUniversel.Utils
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    #endregion

    /// <summary>
    ///     Substitution de jetons dans le corps d'une réponse bouchonnée. Jetons supportés :
    ///     <c>{{route}}</c>, <c>{{query.NOM}}</c>, <c>{{header.NOM}}</c>, <c>{{guid}}</c>,
    ///     <c>{{now}}</c> et <c>{{now:FORMAT}}</c>.
    /// </summary>
    public static class ResponseTemplate
    {
        private static readonly Regex TokenRegex = new (@"\{\{\s*(?<token>[^}]+?)\s*\}\}", RegexOptions.Compiled);

        /// <summary>Applique la substitution des jetons.</summary>
        /// <param name="body">Le corps à transformer.</param>
        /// <param name="route">La route appelée.</param>
        /// <param name="query">Les paramètres de requête (première valeur par clé).</param>
        /// <param name="headers">Les en-têtes (première valeur par clé).</param>
        /// <returns>Le corps avec les jetons remplacés.</returns>
        public static string Apply(
            string body,
            string route,
            IReadOnlyDictionary<string, string> query,
            IReadOnlyDictionary<string, string> headers)
        {
            if (string.IsNullOrEmpty(body))
            {
                return body;
            }

            return TokenRegex.Replace(body, match => Resolve(match.Groups["token"].Value, route, query, headers));
        }

        private static string Resolve(
            string token,
            string route,
            IReadOnlyDictionary<string, string> query,
            IReadOnlyDictionary<string, string> headers)
        {
            if (string.Equals(token, "route", StringComparison.OrdinalIgnoreCase))
            {
                return route ?? string.Empty;
            }

            if (string.Equals(token, "guid", StringComparison.OrdinalIgnoreCase))
            {
                return Guid.NewGuid().ToString();
            }

            if (string.Equals(token, "now", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            }

            if (token.StartsWith("now:", StringComparison.OrdinalIgnoreCase))
            {
                return DateTime.UtcNow.ToString(token["now:".Length..], CultureInfo.InvariantCulture);
            }

            if (token.StartsWith("query.", StringComparison.OrdinalIgnoreCase))
            {
                return Lookup(query, token["query.".Length..]);
            }

            if (token.StartsWith("header.", StringComparison.OrdinalIgnoreCase))
            {
                return Lookup(headers, token["header.".Length..]);
            }

            // Jeton inconnu : on le laisse tel quel.
            return "{{" + token + "}}";
        }

        private static string Lookup(IReadOnlyDictionary<string, string> source, string key)
            => source != null && source.TryGetValue(key, out var value) ? value : string.Empty;
    }
}
