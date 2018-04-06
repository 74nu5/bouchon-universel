namespace BouchonUniversel.Controllers
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;

    using Exceptions;

    using Metier;

    using Microsoft.AspNetCore.Mvc;

    using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;

    #endregion

    /// <summary>The bouchon controller.</summary>
    [Produces("application/json")]
    [Route("api/Bouchon")]
    public class BouchonController : Controller
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

        /// <summary>The get.</summary>
        /// <param name="cle">The cle.</param>
        /// <param name="env">The env.</param>
        /// <param name="route">The route.</param>
        /// <param name="query">The query.</param>
        /// <returns>The Dictionary.</returns>
        [HttpGet("{cle}/{env}/{*route}")]
        public IActionResult Get([FromRoute] string cle, [FromRoute] string env, [FromRoute] string route, [FromQuery] Dictionary<string, string> query)
        {
            try
            {
                var result = this.metier.ProcessRequest(cle, env, route, query);
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

        /// <summary>The post.</summary>
        /// <param name="value">The value.</param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>The put.</summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>The delete.</summary>
        /// <param name="id">The id.</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #endregion
    }
}