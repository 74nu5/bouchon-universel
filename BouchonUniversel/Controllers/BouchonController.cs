namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using BouchonUniversel.Metier;
    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Utils;

    using Microsoft.AspNetCore.Mvc;

    using Ustilz.Http;

    #endregion

    /// <summary>The bouchon controller.</summary>
    [Route("api/bouchon")]
    public sealed class BouchonController : Controller
    {
        /// <summary>The metier.</summary>
        private readonly BouchonsMetier metier;

        /// <summary>Initializes a new instance of the <see cref="BouchonController" /> class.</summary>
        /// <param name="metier">The metier.</param>
        public BouchonController(BouchonsMetier metier)
            => this.metier = metier;

        /// <summary>Méthode get du bouchon (ne gère pas l'envoi d'un body en get).</summary>
        /// <remarks>
        ///     Les headers passés en entrée sont transmis au service associé au bouchon ; les headers renvoyés par le service sont
        ///     écrits dans la réponse.
        /// </remarks>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler (tout ce qu'il y a après l'environnement est pris en compte).</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="404">Si la clé du service ou l'environnement n'a pas été trouvé.</response>
        /// <response code="500">En cas d'erreur inattendue lors du traitement de la requête.</response>
        [HttpGet("{cle}/{env}/{*route}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Get([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            var headers = HttpUtils.GetHeadersFiltered(this.Request.Headers);
            var (result, erreur) = await this.metier.ProcessGetRequestAsync(cle, env, HttpUtility.UrlDecode(route), query, headers).ConfigureAwait(false);
            return this.BuildResponse(result, erreur);
        }

        /// <summary>The healthcheck.</summary>
        /// <returns>The <see cref="string" />.</returns>
        [HttpGet("healthcheck")]
        public string Healthcheck()
            => "OK";

        /// <summary>Méthode post du bouchon.</summary>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler.</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="404">Si la clé du service ou l'environnement n'a pas été trouvé.</response>
        /// <response code="500">En cas d'erreur inattendue lors du traitement de la requête.</response>
        [HttpPost("{cle}/{env}/{*route}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Post([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            var headers = HttpUtils.GetHeadersFiltered(this.Request.Headers);
            var body = await this.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            var (result, erreur) = await this.metier.ProcessPostRequestAsync(cle, env, route, query, headers, body).ConfigureAwait(false);
            return this.BuildResponse(result, erreur);
        }

        /// <summary>Méthode put du bouchon.</summary>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler.</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="404">Si la clé du service ou l'environnement n'a pas été trouvé.</response>
        /// <response code="500">En cas d'erreur inattendue lors du traitement de la requête.</response>
        [HttpPut("{cle}/{env}/{*route}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Put([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            var headers = HttpUtils.GetHeadersFiltered(this.Request.Headers);
            var body = await this.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            var (result, erreur) = await this.metier.ProcessPutRequestAsync(cle, env, route, query, headers, body).ConfigureAwait(false);
            return this.BuildResponse(result, erreur);
        }

        /// <summary>Méthode patch du bouchon.</summary>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler.</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="404">Si la clé du service ou l'environnement n'a pas été trouvé.</response>
        /// <response code="500">En cas d'erreur inattendue lors du traitement de la requête.</response>
        [HttpPatch("{cle}/{env}/{*route}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Patch([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            var headers = HttpUtils.GetHeadersFiltered(this.Request.Headers);
            var body = await this.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            var (result, erreur) = await this.metier.ProcessPatchRequestAsync(cle, env, route, query, headers, body).ConfigureAwait(false);
            return this.BuildResponse(result, erreur);
        }

        /// <summary>Méthode delete du bouchon.</summary>
        /// <param name="cle">La clé du service créé.</param>
        /// <param name="env">L'environnement créé.</param>
        /// <param name="route">La route du service à appeler.</param>
        /// <param name="query">La paramètre de la requête.</param>
        /// <returns>Retourne la réponse du service associé au bouchon si celui-ci n'est pas activé, retourne le bouchon sinon.</returns>
        /// <response code="404">Si la clé du service ou l'environnement n'a pas été trouvé.</response>
        /// <response code="500">En cas d'erreur inattendue lors du traitement de la requête.</response>
        [HttpDelete("{cle}/{env}/{*route}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> Delete([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            var headers = HttpUtils.GetHeadersFiltered(this.Request.Headers);
            var body = await this.Request.GetRawBodyStringAsync().ConfigureAwait(false);
            var (result, erreur) = await this.metier.ProcessDeleteRequestAsync(cle, env, route, query, headers, body).ConfigureAwait(false);
            return this.BuildResponse(result, erreur);
        }

        /// <summary>Construit la réponse HTTP à partir du résultat du métier, en propageant le statut sémantique en cas d'erreur.</summary>
        /// <param name="result">La réponse bouchonnée (peut être nulle en cas d'erreur).</param>
        /// <param name="erreur">L'erreur éventuelle.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        private IActionResult BuildResponse(ReponseBouchonnee result, ResponseErreur erreur)
        {
            if (erreur != null)
            {
                return this.StatusCode((int)erreur.StatusCode, erreur.CodeMessage);
            }

            this.Response.Headers["Site"] = "Bouchon-Universel";
            this.Response.SetHeaders(result.Headers?.ToDictionary(kv => kv.Key, kv => kv.Value.AsEnumerable()));

            return this.StatusCode(result.StatusCode, result.Body.ResolveResponse(this.Response.ContentType));
        }
    }
}
