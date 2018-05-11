namespace BouchonUniversel.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.Exceptions;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Utils.Http;

    using Microsoft.AspNetCore.Mvc;

    using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon controller.</summary>
    [Route("api/Bouchon")]
    public sealed class BouchonController : Controller
    {
        #region Champs

        /// <summary>The metier.</summary>
        private readonly BouchonsMetier metier;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonController"/> class.</summary>
        /// <param name="metier">The metier.</param>
        public BouchonController(BouchonsMetier metier) => this.metier = metier;

        #endregion

        #region Méthodes publiques

        /// <summary>The delete.</summary>
        /// <param name="id">The id.</param>
        [HttpDelete("{id}")]
        public void Delete(int id) => throw new NotImplementedException();

        /// <summary>The get.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <returns>The Dictionary.</returns>
        [HttpGet("{cle}/{env}/{*route}")]
        public async Task<IActionResult> Get(
            [FromRoute] string cle,
            [FromRoute] string env,
            [FromRoute] string route,
            [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            try
            {
                var headers = this.Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
                var result = await this.metier.ProcessGetRequestAsync(cle, env, route, query, headers);
                this.Response.SetHeaders(result.Headers?.ToDictionary(kv => kv.Key, kv => kv.Value.AsEnumerable()));
                return this.StatusCode(result.StatusCode, result.Body);
            }
            catch (KeyNotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (EnvironmentNotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>The post.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost("{cle}/{env}/{*route}")]
        public async Task<IActionResult> Post(
            [FromRoute] string cle,
            [FromRoute] string env,
            [FromRoute] string route,
            [FromQuery] Dictionary<string, IEnumerable<string>> query)
        {
            try
            {
                var headers = this.Request.Headers.ToDictionary(pair => pair.Key, pair => pair.Value.AsEnumerable());
                var result = await this.metier.ProcessPostRequestAsync(cle, env, route, query, headers, await this.Request.GetRawBodyStringAsync());
                return this.Ok(result);
            }
            catch (KeyNotFoundException httpEx)
            {
                return this.NotFound(httpEx.Message);
            }
            catch (EnvironmentNotFoundException httpEx)
            {
                return this.NotFound(httpEx.Message);
            }
        }

        /// <summary>The put.</summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) => throw new NotImplementedException();

        #endregion
    }
}