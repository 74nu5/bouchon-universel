namespace BouchonUniversel.Utils
{
    #region Usings

    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BouchonUniversel.Models.Bouchons;

    using Ustilz.Extensions;

    #endregion

    /// <summary>
    ///     Réalise un appel HTTP de type passthrough (relais vers le service réel) pour les verbes non gérés par Ustilz.Http
    ///     (PUT, DELETE, PATCH). Isolé du métier pour être testable indépendamment.
    /// </summary>
    public static class PassthroughHttp
    {
        /// <summary>Envoie la requête et convertit la réponse en <see cref="ReponseBouchonnee" />.</summary>
        /// <param name="client">Le client HTTP à utiliser.</param>
        /// <param name="httpMethod">Le verbe HTTP.</param>
        /// <param name="url">L'URL cible.</param>
        /// <param name="headers">Les en-têtes à transmettre.</param>
        /// <param name="body">Le corps de la requête (ignoré pour DELETE).</param>
        /// <param name="req">La requête d'origine, conservée dans la réponse.</param>
        /// <returns>The <see cref="ReponseBouchonnee" />.</returns>
        public static async Task<ReponseBouchonnee> SendAsync(
            HttpClient client,
            HttpMethod httpMethod,
            string url,
            Dictionary<string, IEnumerable<string>> headers,
            string body,
            Request req)
        {
            using var request = new HttpRequestMessage(httpMethod, url);

            if (body != null && httpMethod != HttpMethod.Delete)
            {
                request.Content = new StringContent(body);
            }

            if (headers != null)
            {
                foreach (var (key, values) in headers)
                {
                    if (!request.Headers.TryAddWithoutValidation(key, values) && request.Content != null)
                    {
                        request.Content.Headers.TryAddWithoutValidation(key, values);
                    }
                }
            }

            using var response = await client.SendAsync(request).ConfigureAwait(false);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var responseHeaders = new Dictionary<string, IEnumerable<string>>();
            foreach (var (key, value) in response.Headers)
            {
                responseHeaders[key] = value;
            }

            foreach (var (key, value) in response.Content.Headers)
            {
                responseHeaders[key] = value;
            }

            return new ReponseBouchonnee
                   {
                       Body = responseBody,
                       Headers = responseHeaders.ToKeyValueList(),
                       Request = req,
                       StatusCode = (int)response.StatusCode,
                       ResponsePhrase = response.ReasonPhrase,
                   };
        }
    }
}
