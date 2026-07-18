namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Xml.Linq;

    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Exceptions;
    using BouchonUniversel.Models;
    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Models.ModelsView;
    using BouchonUniversel.Services;
    using BouchonUniversel.Utils;

    using JetBrains.Annotations;

    using Microsoft.Extensions.Logging;

    using Ustilz.Extensions;
    using Ustilz.Extensions.String;
    using Ustilz.Http;
    using Ustilz.Json;
    using Ustilz.Xml;

    using KeyNotFoundException = BouchonUniversel.Exceptions.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon metier.</summary>
    [UsedImplicitly]
    public sealed class BouchonsMetier
    {
        /// <summary>The environnement dao.</summary>
        private readonly EnvironnementDAO environnementDAO;

        /// <summary>The http.</summary>
        private readonly HttpService http;

        private readonly ILogger<BouchonsMetier> logger;

        /// <summary>The services dao.</summary>
        private readonly ServicesDAO servicesDAO;

        /// <summary>The settings bouchon dao.</summary>
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        private readonly FileService fileService;

        private readonly PatternDateFormatProvider patternDateFormatProvider;

        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier" /> class.</summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        /// <param name="settingsBouchonDAO">The settings Bouchon DAO.</param>
        /// <param name="http">The http.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="patternDateFormatProvider">Fournit (en cache) la configuration des patterns de dates.</param>
        /// <param name="httpClientFactory">Fabrique de clients HTTP pour les verbes PUT/DELETE/PATCH (non gérés par Ustilz.Http).</param>
        public BouchonsMetier(HttpService http, ILogger<BouchonsMetier> logger, ServicesDAO servicesDAO, EnvironnementDAO environnementDAO, SettingsBouchonDAO settingsBouchonDAO, FileService fileService, PatternDateFormatProvider patternDateFormatProvider, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
            this.settingsBouchonDAO = settingsBouchonDAO;
            this.fileService = fileService;
            this.patternDateFormatProvider = patternDateFormatProvider;
            this.httpClientFactory = httpClientFactory;
            this.http = http;
        }

        /// <summary>The get files of service.</summary>
        /// <param name="service">The service.</param>
        /// <returns>The <see cref="DirectoryBouchon" />.</returns>
        internal DirectoryBouchon GetFilesOfService(Service service)
        {
            var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), service.Cle, service.Environnement.Nom));
            return !bouchonDir.Exists ? new () : this.GetFileAndDirectory(bouchonDir);
        }

        /// <summary>The process request.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="ReponseBouchonnee" />.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur? erreur)> ProcessGetRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers)
            => await this.ProcessRequestAsync(HttpVerb.Get, cle, env, route, query, headers, null).ConfigureAwait(false);

        /// <summary>The process post request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="Task" />.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur? erreur)> ProcessPostRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
            => await this.ProcessRequestAsync(HttpVerb.Post, cle, env, route, query, headers, body).ConfigureAwait(false);

        /// <summary>The process put request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task" />.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur? erreur)> ProcessPutRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
            => await this.ProcessRequestAsync(HttpVerb.Put, cle, env, route, query, headers, body).ConfigureAwait(false);

        /// <summary>The process patch request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task" />.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur? erreur)> ProcessPatchRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
            => await this.ProcessRequestAsync(HttpVerb.Patch, cle, env, route, query, headers, body).ConfigureAwait(false);

        /// <summary>The process delete request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task" />.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur? erreur)> ProcessDeleteRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
            => await this.ProcessRequestAsync(HttpVerb.Delete, cle, env, route, query, headers, body).ConfigureAwait(false);

        /// <summary>The format if date.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="string" />.</returns>
        private static string FormatIfDate(string value)
            => DateTime.TryParse(value, out var _) ? string.Empty : value;

        /// <summary>The get file and directory.</summary>
        /// <param name="dir">The dir.</param>
        /// <returns>The <see cref="DirectoryBouchon" />.</returns>
        private DirectoryBouchon GetFileAndDirectory(DirectoryInfo dir)
        {
            var result = new DirectoryBouchon
                         {
                             Name = dir.Name,
                             FileBouchons = dir.GetFileSystemInfos().Select(file => new FileBouchon { Name = file.Name, FullName = file.FullName }).ToList(),
                             Directories = dir.GetDirectories().Select(this.GetFileAndDirectory).ToList()
                         };
            return result;
        }

        /// <summary>The process request async.</summary>
        /// <param name="method">The get.</param>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task<(ReponseBouchonnee? reponse, ResponseErreur? erreur)> ProcessRequestAsync(
            HttpVerb method,
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
        {
            try
            {
                var req = new Request { Headers = headers.ToKeyValueList(), Query = query.ToKeyValueList(), Route = route, Body = body };
                var requestIsActivated = await this.ServiceIsActivatedAsync(cle, env).ConfigureAwait(false);

                /* Ingénierie du chaos : latence et injection d'erreurs configurées sur le service. */
                var chaosResponse = await this.ApplyChaosAsync(cle, env, req).ConfigureAwait(false);
                if (chaosResponse != null)
                {
                    return (chaosResponse, null);
                }

                /* Intégration de la mise à jour de la réponse */
                var updateDatesResponseIsActivated = await this.UpdateDatesForServiceIsActivatedAsync(cle).ConfigureAwait(false);

                /* =========================================== */
                var bouchonDir = this.fileService.CreateBouchonDirectory(cle, env, route);

                var queryStr = string.Join(
                                           "&",
                                           query.Select(pair => string.Join("&", pair.Value.Select(value => $"{pair.Key}={HttpUtility.UrlEncode(value)}").ToArray())).ToArray());

                var fileName = GetFileName(method, query, bouchonDir);

                var pdfc = this.patternDateFormatProvider.Config;

                if (requestIsActivated)
                {
                    var responseBouchonne = XDocument.Load(fileName).FromXml<ReponseBouchonnee>();

                    if (updateDatesResponseIsActivated)
                    {
                        responseBouchonne.Body = responseBouchonne.Body.AjustDates(DateTime.Now.ToString(CultureInfo.CurrentCulture), pdfc.Patterns);
                    }

                    return (responseBouchonne, null);
                }

                var urlBase = await this.servicesDAO.GetUrlAsync(cle, env).ConfigureAwait(false);
                var url = new Uri(urlBase, new Uri(route + (!string.IsNullOrEmpty(queryStr) ? $"?{queryStr}" : string.Empty), UriKind.Relative));

                var reponse = default(ReponseBouchonnee);
                switch (method)
                {
                    case HttpVerb.Get:
                    {
                        var (httpCode, responsePhrase, responseHeaders, response) = await this.http.GetHttpResponseAsync(url.ToString(), headers, null).ConfigureAwait(false);
                        reponse = new ()
                                  {
                                      Body = response,
                                      Headers = responseHeaders.ToKeyValueList(),
                                      Request = req,
                                      StatusCode = (int)httpCode,
                                      ResponsePhrase = responsePhrase
                                  };
                        break;
                    }

                    case HttpVerb.Put:
                        reponse = await this.SendViaHttpClientAsync(HttpMethod.Put, url.ToString(), headers, body, req).ConfigureAwait(false);
                        break;
                    case HttpVerb.Delete:
                        reponse = await this.SendViaHttpClientAsync(HttpMethod.Delete, url.ToString(), headers, body, req).ConfigureAwait(false);
                        break;
                    case HttpVerb.Post:
                    {
                        var (httpCode, responsePhrase, responseHeaders, response) = await this.http.PostHttpResponseAsync(url.ToString(), headers, body, null).ConfigureAwait(false);
                        reponse = new ()
                                  {
                                      Body = response,
                                      Headers = responseHeaders.ToKeyValueList(),
                                      Request = req,
                                      StatusCode = (int)httpCode,
                                      ResponsePhrase = responsePhrase
                                  };
                        break;
                    }

                    case HttpVerb.Head:

                        // TODO Gérer le verbe HEAD
                        break;
                    case HttpVerb.Trace:

                        // TODO Gérer le verbe TRACE
                        break;
                    case HttpVerb.Patch:
                        reponse = await this.SendViaHttpClientAsync(HttpMethod.Patch, url.ToString(), headers, body, req).ConfigureAwait(false);
                        break;
                    case HttpVerb.Connect:

                        // TODO Gérer le verbe CONNECT
                        break;
                    case HttpVerb.Options:

                        // TODO Gérer le verbe OPTIONS
                        break;
                    case HttpVerb.Custom:

                        // TODO Gérer le verbe CUSTOM
                        break;
                    case HttpVerb.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(method), method, null);
                }

                await File.WriteAllTextAsync(fileName, reponse?.ToXml()).ConfigureAwait(false);

                if (updateDatesResponseIsActivated && reponse != null)
                {
                    reponse.Body = reponse.Body.AjustDates(DateTime.Now.ToString(CultureInfo.CurrentCulture), pdfc.Patterns);
                }

                return (reponse, null);
            }
            catch (FileNotFoundException ex)
            {
                this.logger.LogError(ex, "Erreur lors de la requete");
                var confDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), cle, env, string.Empty));
                var (reponse, reponseBouchonnee) = MockRealTime.GetUpdatedResponse(confDir.FullName, route);

                var req = new Request { Headers = headers.ToKeyValueList(), Query = query.ToKeyValueList(), Route = route, Body = body };
                var newResponseBouchon = new ReponseBouchonnee
                                         {
                                             Body = reponse,
                                             Headers = reponseBouchonnee.Headers,
                                             Request = req,
                                             StatusCode = reponseBouchonnee.StatusCode,
                                             ResponsePhrase = reponseBouchonnee.ResponsePhrase
                                         };
                return (newResponseBouchon, null);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Erreur lors de la requete");
                return (null, ResponseErreur.FromException(ex));
            }
        }

        /// <summary>Effectue un appel de type passthrough via <see cref="HttpClient" /> pour les verbes non gérés par Ustilz.Http (PUT/DELETE/PATCH).</summary>
        /// <param name="httpMethod">Le verbe HTTP à utiliser.</param>
        /// <param name="url">L'URL cible.</param>
        /// <param name="headers">Les en-têtes à transmettre.</param>
        /// <param name="body">Le corps de la requête (ignoré pour DELETE).</param>
        /// <param name="req">La requête d'origine, conservée dans la réponse enregistrée.</param>
        /// <returns>The <see cref="ReponseBouchonnee" />.</returns>
        private async Task<ReponseBouchonnee> SendViaHttpClientAsync(
            HttpMethod httpMethod,
            string url,
            Dictionary<string, IEnumerable<string>> headers,
            string body,
            Request req)
            => await PassthroughHttp.SendAsync(this.httpClientFactory.CreateClient(), httpMethod, url, headers, body, req).ConfigureAwait(false);

        /// <summary>Applique la latence simulée puis, selon la probabilité configurée, retourne une réponse d'erreur injectée.</summary>
        /// <param name="cle">La clé du service.</param>
        /// <param name="env">L'environnement.</param>
        /// <param name="req">La requête d'origine.</param>
        /// <returns>Une <see cref="ReponseBouchonnee" /> d'erreur si le chaos l'impose ; sinon <c>null</c>.</returns>
        private async Task<ReponseBouchonnee> ApplyChaosAsync(string cle, string env, Request req)
        {
            var service = await this.servicesDAO.GetByCleEnvAsync(cle, env).ConfigureAwait(false);
            if (service == null)
            {
                return null;
            }

            if (service.LatencyMs > 0)
            {
                await Task.Delay(service.LatencyMs).ConfigureAwait(false);
            }

            if (!ChaosPlan.ShouldInjectError(service.ErrorProbability, Random.Shared.Next(100)))
            {
                return null;
            }

            return new ReponseBouchonnee
                   {
                       Body = "Erreur simulée (chaos).",
                       Headers = new List<KeyValue>(),
                       Request = req,
                       StatusCode = ChaosPlan.ResolveErrorStatusCode(service.ErrorStatusCode),
                       ResponsePhrase = "Chaos",
                   };
        }

        private static string GetFileName(HttpVerb method, Dictionary<string, IEnumerable<string>> query, FileSystemInfo bouchonDir)
        {
            var queryHash = string.Join(
                                        "&",
                                        query.Select(pair => string.Join("&", pair.Value.Select(value => $"{pair.Key}={HttpUtility.UrlEncode(FormatIfDate(value))}").ToArray()))
                                             .ToArray())
                                  .ComputeHash(ExtensionsString.HashType.SHA256);

            return $"{Path.Combine(bouchonDir.FullName, $"{method}_{queryHash}")}.xml";
        }

        /// <summary>The assert service exists.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="bool" />.</returns>
        private async Task<bool> ServiceIsActivatedAsync(string cle, string env)
        {
            if (!await this.servicesDAO.ExistsByCleAsync(cle).ConfigureAwait(false))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            return await this.environnementDAO.ExistsByNameAsync(env).ConfigureAwait(false)
                       ? await this.servicesDAO.IsActivated(cle).ConfigureAwait(false) && await this.environnementDAO.IsActivatedAsync(env).ConfigureAwait(false)
                       : throw new EnvironmentNotFoundException("L'environnement n'existe pas");
        }

        /// <summary>Assert that update dates is activated.</summary>
        /// <param name="cle">The cle.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <returns>The <see cref="bool" />.</returns>
        private async Task<bool> UpdateDatesForServiceIsActivatedAsync(string cle)
        {
            if (!await this.servicesDAO.ExistsByCleAsync(cle).ConfigureAwait(false))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            return await this.servicesDAO.IsEnabledToUpdateDatesAsync(cle).ConfigureAwait(false);
        }
    }
}
