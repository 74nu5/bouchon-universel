namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
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

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier" /> class.</summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        /// <param name="settingsBouchonDAO">The settings Bouchon DAO.</param>
        /// <param name="http">The http.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="patternDateFormatProvider">Fournit (en cache) la configuration des patterns de dates.</param>
        public BouchonsMetier(HttpService http, ILogger<BouchonsMetier> logger, ServicesDAO servicesDAO, EnvironnementDAO environnementDAO, SettingsBouchonDAO settingsBouchonDAO, FileService fileService, PatternDateFormatProvider patternDateFormatProvider)
        {
            this.logger = logger;
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
            this.settingsBouchonDAO = settingsBouchonDAO;
            this.fileService = fileService;
            this.patternDateFormatProvider = patternDateFormatProvider;
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

                        // TODO Gérer le verbe PUT
                        break;
                    case HttpVerb.Delete:

                        // TODO Gérer le verbe DELETE
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

                        // TODO Gérer le verbe PATCH
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
            catch (KeyNotFoundException ex)
            {
                this.logger.LogError(ex, "Erreur lors de la requete");
                return (null, new () { Message = ex.Message, Code = 1001 });
            }
            catch (EnvironmentNotFoundException ex)
            {
                this.logger.LogError(ex, "Erreur lors de la requete");
                return (null, new () { Message = ex.Message, Code = 1002 });
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
                return (null, new () { Message = ex.Message, Code = 1999 });
            }
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
