namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
    using BouchonUniversel.Utils;
    using BouchonUniversel.Utils.Extensions;
    using BouchonUniversel.Utils.Http;
    using BouchonUniversel.Utils.Json;
    using BouchonUniversel.Utils.Xml;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

    using KeyNotFoundException = BouchonUniversel.Exceptions.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon metier.</summary>
    [UsedImplicitly]
    [SuppressMessage("ReSharper", "StyleCop.SA1008", Justification = "Stylecop Issue with Tuple")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class BouchonsMetier
    {
        #region Champs

        /// <summary>The environnement dao.</summary>
        private readonly EnvironnementDAO environnementDAO;

        /// <summary>The http.</summary>
        private readonly HttpService http;

        /// <summary>The services dao.</summary>
        private readonly ServicesDAO servicesDAO;

        /// <summary>The settings bouchon dao.</summary>
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsMetier"/> class.</summary>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        /// <param name="settingsBouchonDAO">The settings Bouchon DAO.</param>
        /// <param name="http">The http.</param>
        public BouchonsMetier(ServicesDAO servicesDAO, EnvironnementDAO environnementDAO, SettingsBouchonDAO settingsBouchonDAO, HttpService http)
        {
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
            this.settingsBouchonDAO = settingsBouchonDAO;
            this.http = http;
        }

        #endregion

        #region Méthodes internes

        /// <summary>The get files of service.</summary>
        /// <param name="service">The service.</param>
        /// <returns>The <see cref="DirectoryBouchon"/>.</returns>
        internal DirectoryBouchon GetFilesOfService(Service service)
        {
            var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), service.Cle, service.Environnement.Nom));
            return !bouchonDir.Exists ? new DirectoryBouchon() : this.GetFileAndDirectory(bouchonDir);
        }

        /// <summary>The process request.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="ReponseBouchonnee"/>.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur erreur)> ProcessGetRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers)
            => await this.ProcessRequestAsync(HttpMethod.Get, cle, env, route, query, headers, null);

        /// <summary>The process post request async.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="body">The body.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="Task"/>.</returns>
        internal async Task<(ReponseBouchonnee reponse, ResponseErreur erreur)> ProcessPostRequestAsync(
            string cle,
            string env,
            string route,
            Dictionary<string, IEnumerable<string>> query,
            Dictionary<string, IEnumerable<string>> headers,
            string body)
            => await this.ProcessRequestAsync(HttpMethod.Post, cle, env, route, query, headers, body);

        #endregion

        #region Méthodes privées

        /// <summary>The format if date.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string FormatIfDate(string value)
            => DateTime.TryParse(value, out _) ? string.Empty : value;

        /// <summary>The get file and directory.</summary>
        /// <param name="dir">The dir.</param>
        /// <returns>The <see cref="DirectoryBouchon"/>.</returns>
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
        /// <returns>The <see cref="Task"/>.</returns>
        private async Task<(ReponseBouchonnee reponse, ResponseErreur erreur)> ProcessRequestAsync(
            HttpMethod method,
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
                var requestIsActivated = this.ServiceIsActivated(cle, env);

                /* Intégration de la mise à jour de la réponse */
                var updateDatesResponseIsActivated = this.UpdateDatesForServiceIsActivated(cle);

                /* =========================================== */
                var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), cle, env, route ?? string.Empty));
                if (!bouchonDir.Exists)
                {
                    bouchonDir.Create();
                }

                var queryStr = string.Join(
                    "&", query.Select(pair => string.Join("&", pair.Value.Select(value => $"{pair.Key}={HttpUtility.UrlEncode(value)}").ToArray())).ToArray());

                var queryHash = string.Join(
                        "&", query.Select(pair => string.Join("&", pair.Value.Select(value => $"{pair.Key}={HttpUtility.UrlEncode(FormatIfDate(value))}").ToArray())).ToArray())
                    .ComputeHash(ExtensionsString.HashType.SHA256);

                var fileName = $"{Path.Combine(bouchonDir.FullName, $"{method.ToString()}_{queryHash}")}.xml";

                var pdfc = File.ReadAllText(@"./PatternDateFormatConfig.json").FromJson<PatternDateFormatConfig>();

                if (requestIsActivated)
                {
                    var responseBouchonne = XDocument.Load(fileName).FromXml<ReponseBouchonnee>();
                    if (updateDatesResponseIsActivated)
                    {
                        responseBouchonne.Body = responseBouchonne.Body.AjustDates(DateTime.Now.ToString(CultureInfo.CurrentCulture), pdfc.Patterns);
                    }

                    return (responseBouchonne, null);
                }

                var urlBase = this.servicesDAO.GetUrl(cle, env);
                var url = new Uri(urlBase, new Uri(route + (!string.IsNullOrEmpty(queryStr) ? $"?{queryStr}" : string.Empty), UriKind.Relative));

                var reponse = default(ReponseBouchonnee);
                switch (method)
                {
                    case HttpMethod.Get:
                    {
                        var (httpCode, responsePhrase, responseHeaders, response) = await this.http.GetHttpResponseAsync(url.ToString(), headers, null);
                        reponse = new ReponseBouchonnee
                        {
                            Body = response,
                            Headers = responseHeaders.ToKeyValueList(),
                            Request = req,
                            StatusCode = (int)httpCode,
                            ResponsePhrase = responsePhrase
                        };
                        break;
                    }

                    case HttpMethod.Put:

                        // TODO Gérer le verbe PUT
                        break;
                    case HttpMethod.Delete:

                        // TODO Gérer le verbe DELETE
                        break;
                    case HttpMethod.Post:
                    {
                        var (httpCode, responsePhrase, responseHeaders, response) = await this.http.PostHttpResponseAsync(url.ToString(), headers, body, null);
                        reponse = new ReponseBouchonnee
                        {
                            Body = response,
                            Headers = responseHeaders.ToKeyValueList(),
                            Request = req,
                            StatusCode = (int)httpCode,
                            ResponsePhrase = responsePhrase
                        };
                        break;
                    }

                    case HttpMethod.Head:

                        // TODO Gérer le verbe HEAD
                        break;
                    case HttpMethod.Trace:

                        // TODO Gérer le verbe TRACE
                        break;
                    case HttpMethod.Patch:

                        // TODO Gérer le verbe PATCH
                        break;
                    case HttpMethod.Connect:

                        // TODO Gérer le verbe CONNECT
                        break;
                    case HttpMethod.Options:

                        // TODO Gérer le verbe OPTIONS
                        break;
                    case HttpMethod.Custom:

                        // TODO Gérer le verbe CUSTOM
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(method), method, null);
                }

                File.WriteAllText(fileName, reponse?.ToXml());

                if (updateDatesResponseIsActivated && reponse != null)
                {
                    reponse.Body = reponse.Body.AjustDates(DateTime.Now.ToString(CultureInfo.CurrentCulture), pdfc.Patterns);
                }

                return (reponse, null);
            }
            catch (KeyNotFoundException ex)
            {
                return (null, new ResponseErreur { Message = ex.Message, Code = 1001 });
            }
            catch (EnvironmentNotFoundException ex)
            {
                return (null, new ResponseErreur { Message = ex.Message, Code = 1002 });
            }
            catch (FileNotFoundException ex)
            {
                var confDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), cle, env, string.Empty));
                var newResponse = MockRealTime.GetUpdatedResponse(confDir.FullName, route);
                if (!newResponse.Item2.IsNull())
                {
                    var req = new Request { Headers = headers.ToKeyValueList(), Query = query.ToKeyValueList(), Route = route, Body = body };
                    var newResponseBouchon = new ReponseBouchonnee
                    {
                        Body = newResponse.Item1,
                        Headers = newResponse.Item2.Headers,
                        Request = req,
                        StatusCode = newResponse.Item2.StatusCode,
                        ResponsePhrase = newResponse.Item2.ResponsePhrase
                    };
                    return (newResponseBouchon, null);
                }

                return (null, new ResponseErreur { Message = ex.Message, Code = 1003 });
            }
            catch (Exception ex)
            {
                return (null, new ResponseErreur { Message = ex.Message, Code = 1999 });
            }
        }

        /// <summary>The assert service exists.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <exception cref="EnvironmentNotFoundException">Lève une exception si l'environnement n'existe pas.</exception>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ServiceIsActivated(string cle, string env)
        {
            if (!this.servicesDAO.ExistsByCle(cle))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            if (!this.environnementDAO.ExistsByName(env))
            {
                throw new EnvironmentNotFoundException("L'environnement n'existe pas");
            }

            return this.servicesDAO.IsActivated(cle) && this.environnementDAO.IsActivated(env);
        }

        /// <summary>Assert that update dates is activated.</summary>
        /// <param name="cle">The cle.</param>
        /// <exception cref="Exceptions.KeyNotFoundException">Lève une exception si la clé n'existe pas.</exception>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool UpdateDatesForServiceIsActivated(string cle)
        {
            if (!this.servicesDAO.ExistsByCle(cle))
            {
                throw new KeyNotFoundException("La clé n'existe pas");
            }

            return this.servicesDAO.IsEnabledToUpdateDates(cle);
        }

        #endregion
    }
}