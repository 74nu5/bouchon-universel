namespace BouchonUniversel.Controllers {
    #region Usings

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using System;
    using Metier;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Newtonsoft.Json;
    using Utils.Http;

    #endregion

    /// <summary>The bouchon controller.</summary>
    [Route ("api/bouchon")]
    [SuppressMessage ("ReSharper", "StyleCop.SA1008", Justification = "Stylecop Issue with Tuple")]
    public sealed class BouchonController : Controller {
        #region Champs

        /// <summary>The metier.</summary>
        private readonly BouchonsMetier metier;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonController"/> class.</summary>
        /// <param name="metier">The metier.</param>
        public BouchonController (BouchonsMetier metier) => this.metier = metier;

        #endregion

        #region Méthodes publiques

        /// <summary>La méthode DELETE n'est pas implémentée.</summary>
        /// <param name="id">The id.</param>
        [HttpDelete ("NotImplemented")]
        public void Delete (int? id) =>
            throw new NotImplementedException ();

        /// <summary>Méthode get du bouchon (ne gère pas l'envoi d'un body en get).</summary>
        /// <remarks>Les headers passés en entrée sont transmis au service associé au bouchon ; les headers renvoyés par le service sont
        ///     écrits dans la réponse.</remarks>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler (tout ce qu'il y a après l'environnement est pris en compte).</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="405">
        ///     -   Si la clé du service n'a pas été trouvée (un méssage spécifique est associé au retour)
        ///     -   Si l'environnement n'a pas été trouve (un méssage spécifique est associé au retour)
        ///     -   Si le fichier de bouchon n'a pas été trouvé (un méssage spécifique est associé au retour)
        ///     -   Si le service associé au bouchon répond 405
        /// </response>
        [HttpGet ("{cle}/{env}/{*route}")]
        [ProducesResponseType (typeof (string), 200)]
        [ProducesResponseType (typeof (string), 405)]
        public async Task<IActionResult> Get ([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query) {
            var headers = this.Request.Headers.ToDictionary (pair => pair.Key, pair => pair.Value.AsEnumerable ());
            var (result, erreur) = await this.metier.ProcessGetRequestAsync (cle, env, HttpUtility.UrlDecode (route), query, headers);

            if (erreur != null) {
                return this.StatusCode ((int) HttpStatusCode.MethodNotAllowed, erreur.CodeMessage);
            }

            this.Response.Headers.Add ("Content-Type", "application/json");
            this.Response.Headers.Add ("Site", "Bouchon-Universel");
            this.Response.SetHeaders (result.Headers?.ToDictionary (kv => kv.Key, kv => kv.Value.AsEnumerable ()));

            Console.WriteLine (this.Response.Headers);

            return this.StatusCode (result.StatusCode, result.Body.ResolveResponse (this.Response.ContentType));
        }

        /// <summary>The healthcheck.</summary>
        /// <returns>The <see cref="string" />.</returns>
        [HttpGet ("healthcheck")]
        public string Healthcheck () => "OK";

        /// <summary>Méthode post du bouchon.</summary>
        /// <remarks>Les headers passés en entrée sont transmis au service associé au bouchon ; les headers renvoyés par le service sont
        ///     écrits dans la réponse.</remarks>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler (tout ce qu'il y a après l'environnement est pris en compte).</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="405">
        ///     -   Si la clé du service n'a pas été trouvée (un méssage spécifique est associé au retour)
        ///     -   Si l'environnement n'a pas été trouve (un méssage spécifique est associé au retour)
        ///     -   Si le fichier de bouchon n'a pas été trouvé (un méssage spécifique est associé au retour)
        ///     -   Si le service associé au bouchon répond 405
        /// </response>
        [HttpPost ("{cle}/{env}/{*route}")]
        public async Task<IActionResult> Post ([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query) {
            var headers = this.Request.Headers.ToDictionary (pair => pair.Key, pair => pair.Value.AsEnumerable ());

            // On récupère le body de cette façon pour prendre en compte tous les genres de body (texte, json, binaires, ...).
            var (result, erreur) = await this.metier.ProcessPostRequestAsync (cle, env, route, query, headers, await this.Request.GetRawBodyStringAsync ());

            if (erreur != null) {
                return this.StatusCode ((int) HttpStatusCode.MethodNotAllowed, erreur.CodeMessage);
            }

            this.Response.Headers.Add ("Site", "Bouchon-Universel");
            this.Response.SetHeaders (result.Headers?.ToDictionary (kv => kv.Key, kv => kv.Value.AsEnumerable ()));

            return this.StatusCode (result.StatusCode, result.Body.ResolveResponse (this.Response.ContentType));
        }

        /// <summary>La méthode PUT n'est pas implémentée.</summary>
        /// <param name="value">The value.</param>
        [HttpPut ("NotImplemented")]
        public void Put ([FromBody] string value) =>
            throw new NotImplementedException ();

        #endregion
    }
}