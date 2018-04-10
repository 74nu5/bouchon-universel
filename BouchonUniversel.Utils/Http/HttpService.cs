namespace BouchonUniversel.Utils.Http
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BouchonUniversel.Utils.Extensions;
    using BouchonUniversel.Utils.Json;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The http service.</summary>
    [PublicAPI]
    public class HttpService
    {
        #region Champs

        /// <summary>The handler.</summary>
        private readonly HttpClientHandler handler;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="HttpService" /> class.</summary>
        public HttpService()
        {
            this.handler = new HttpClientHandler();
            this.handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The delete.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The TResponse.</returns>
        public static TResponse Delete<TResponse>(string url, string authentification) => throw new NotImplementedException();

        /// <summary>The put.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The TResponse.</returns>
        public static TResponse Put<TResponse>(string url, string authentification) => throw new NotImplementedException();

        /// <summary>The get async.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<TResponse> GetAsync<TResponse>(string url, string authentification)
        {
            var result = await new Func<string, string, Task<TResponse>>(this.GetAsyncInternal<TResponse>).TestPerf(out var timestamp, url, authentification);
            Debug.WriteLine($"GET {url} : {timestamp} ms");
            return result;
        }

        /// <summary>The get.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<string> GetAsync(string url, string authentification)
        {
            var result = await new Func<string, Dictionary<string, string[]>, string, Task<string>>(this.GetAsyncInternal).TestPerf(out var timestamp, url, null, authentification);
            Debug.WriteLine($"GET {url} : {timestamp} ms");
            return result;
        }

        /// <summary>The get async.</summary>
        /// <param name="url">The url.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="authentification">The authentification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<string> GetAsync(string url, Dictionary<string, string[]> headers, string authentification)
        {
            var result = await new Func<string, Dictionary<string, string[]>, string, Task<string>>(this.GetAsyncInternal).TestPerf(out var timestamp, url, headers, authentification);
            Debug.WriteLine($"GET {url} : {timestamp} ms");
            return result;
        }

        /// <summary>The get.</summary>
        /// <param name="url">The url.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The TResponse.</returns>
        public async Task<TResponse> GetAsyncInternal<TResponse>(string url, string authentification)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var client = new HttpClient(this.handler);

            client.SetAuthentication(authentification);

            try
            {
                var response = await client.GetStringAsync(url);
                return response.FromJson<TResponse>();
            }
            catch (WebException ex)
            {
                throw ex.ProcessWebException();
            }
        }

        /// <summary>The post.</summary>
        /// <param name="url">The url.</param>
        /// <param name="content">The http content.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Type de la réponse</typeparam>
        /// <returns>The TResponse.</returns>
        public async Task<TResponse> PostAsync<TResponse>(string url, string content, string authentification)
        {
            var result = await new Func<string, Dictionary<string, string[]>, string, string, Task<TResponse>>(this.PostAsyncInternal<TResponse>).TestPerf(
                             out var time, url, null, content, authentification);
            Debug.WriteLine($"GET {url} : {time} ms");
            return result;
        }

        /// <summary>The post async.</summary>
        /// <param name="url">The url.</param>
        /// <param name="content">The content.</param>
        /// <param name="authentification">The authentification.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public async Task<string> PostAsync(string url, string content, string authentification)
        {
            var result = await new Func<string, Dictionary<string, string[]>, string, string, Task<string>>(this.PostAsyncInternal).TestPerf(out var time, url, null, content, authentification);
            Debug.WriteLine($"GET {url} : {time} ms");
            return result;
        }

        #endregion

        #region Méthodes privées

        /// <summary>The get async internal.</summary>
        /// <param name="url">The url.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="authentification">The authentification.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<string> GetAsyncInternal(string url, Dictionary<string, string[]> headers, string authentification)
        {
            var client = new HttpClient(this.handler);

            client.SetAuthentication(authentification);
            client.SetHeaders(headers);

            try
            {
                return await client.GetStringAsync(url);
            }
            catch (WebException ex)
            {
                throw ex.ProcessWebException();
            }
        }

        /// <summary>The post async.</summary>
        /// <param name="url">The url.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <param name="authentication">The authentication.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<string> PostAsyncInternal(string url, Dictionary<string, string[]> headers, string body, string authentication)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var client = new HttpClient(this.handler);

            var content = new StringContent(body);
            client.SetAuthentication(authentication);
            client.SetHeaders(headers);

            try
            {
                var responseMessage = await client.PostAsync(url, content);
                return await responseMessage.Content.ReadAsStringAsync();
            }
            catch (WebException ex)
            {
                throw ex.ProcessWebException();
            }
            finally
            {
                stopWatch.Stop();
                Debug.WriteLine($"POST {url} : {stopWatch.ElapsedMilliseconds} ms");
            }
        }

        /// <summary>The post async internal.</summary>
        /// <param name="url">The url.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="content">The content.</param>
        /// <param name="authentification">The authentification.</param>
        /// <typeparam name="TResponse">Test de la réponse</typeparam>
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<TResponse> PostAsyncInternal<TResponse>(string url, Dictionary<string, string[]> headers, string content, string authentification)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var json = new StringContent(content);

            var client = new HttpClient(this.handler);

            client.SetAuthentication(authentification);

            try
            {
                var response = await client.PostAsync(url, json);
                return response.Content.ToString().FromJson<TResponse>();
            }
            catch (WebException ex)
            {
                throw ex.ProcessWebException();
            }
            finally
            {
                stopWatch.Stop();
                Debug.WriteLine($"POST {url} : {stopWatch.ElapsedMilliseconds} ms");
            }
        }

        #endregion
    }
}