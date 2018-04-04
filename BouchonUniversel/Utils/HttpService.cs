namespace BouchonUniversel.Utils
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The http service.</summary>
    [PublicAPI]
    public sealed class HttpService
    {
        #region Méthodes publiques

        /// <summary>The delete.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The <see cref="TResponse"/>.</returns>
        public static TResponse Delete<TResponse>(string url, string authentification) => throw new NotImplementedException();

        /// <summary>The get.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The <see cref="TResponse"/>.</returns>
        public static TResponse Get<TResponse>(string url, string authentification)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var handler = new HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;

            var client = new HttpClient(handler);

            if (!string.IsNullOrEmpty(authentification))
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authentification);
            }

            try
            {
                var response = client.GetStringAsync(url);
                return response.Result.FromJson<TResponse>();
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;

                using (var responseStream = errorResponse.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw;
                    }

                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));

                    var errorText = reader.ReadToEnd();

                    throw new Exception(errorText);
                }
            }
            finally
            {
                stopWatch.Stop();
                Debug.WriteLine($"GET {url} : {stopWatch.ElapsedMilliseconds} ms");
            }
        }


        /// <summary>The post.</summary>
        /// <param name="url">The url.</param>
        /// <param name="content">The http content.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The <see cref="TResponse"/>.</returns>
        public static async Task<TResponse> Post<TResponse>(string url, string content, string authentification)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var handler = new HttpClientHandler();
            var json = new StringContent(content);

            handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;

            var client = new HttpClient(handler);

            if (!string.IsNullOrEmpty(authentification))
            {
                client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authentification);
            }

            try
            {
                var response = await client.PostAsync(url, json);
                return response.Content.ToString().FromJson<TResponse>();
            }
            catch (WebException ex)
            {
                var errorResponse = ex.Response;

                using (var responseStream = errorResponse.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        throw;
                    }

                    var reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));

                    var errorText = reader.ReadToEnd();

                    throw new Exception(errorText);
                }
            }
            finally
            {
                stopWatch.Stop();
                Debug.WriteLine($"POST {url} : {stopWatch.ElapsedMilliseconds} ms");
            }
        }

        /// <summary>The put.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The <see cref="TResponse"/>.</returns>
        public static TResponse Put<TResponse>(string url, string authentification) => throw new NotImplementedException();

        #endregion
    }
}