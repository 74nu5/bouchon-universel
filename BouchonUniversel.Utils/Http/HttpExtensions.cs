namespace BouchonUniversel.Utils.Http
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    #endregion

    /// <summary>The http client extensions.</summary>
    public static class HttpExtensions
    {
        #region Méthodes publiques

        /// <summary>The set authentication.</summary>
        /// <param name="client">The client.</param>
        /// <param name="authentication">The authentication.</param>
        public static void SetAuthentication(this HttpClient client, string authentication)
        {
            if (!string.IsNullOrEmpty(authentication))
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authentication);
            }
        }

        /// <summary>The set headers.</summary>
        /// <param name="client">The client.</param>
        /// <param name="headers">The headers.</param>
        public static void SetHeaders(this HttpClient client, Dictionary<string, string[]> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        /// <summary>The process web exception.</summary>
        /// <param name="exception">The exception.</param>
        /// <returns>The <see cref="System.Exception"/>.</returns>
        public static Exception ProcessWebException(this WebException exception)
        {
            var errorResponse = exception.Response;

            using (var responseStream = errorResponse.GetResponseStream())
            {
                if (responseStream == null)
                {
                    throw exception;
                }

                var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));

                var errorText = reader.ReadToEnd();

                throw new Exception(errorText);
            }
        }

        #endregion
    }
}