namespace BouchonUniversel.ApiTest.Controllers
{
    #region Usings

    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <summary>The values controller.</summary>
    [Route("api/[controller]")]
    public sealed class ValuesController : Controller
    {
        #region Méthodes publiques

        /// DELETE api/values/5
        /// <summary>The delete.</summary>
        /// <param name="id">The id.</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        /// GET api/values
        /// <summary>The get.</summary>
        /// <returns>The IEnumerable />.</returns>
        [HttpGet]
        public IEnumerable<string> Get() => new[] { "value1", "value2" };

        /// GET api/values/5
        /// <summary>The get.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [HttpGet("{id}")]
        public string Get(int id) => "value";

        /// POST api/values
        /// <summary>The post.</summary>
        /// <returns>The <see cref="OkObjectResult"/>.</returns>
        [HttpPost]
        public async Task<OkObjectResult> PostAsync()
        {
            using (var reader = new StreamReader(this.Request.Body, Encoding.UTF8))
            {
                return this.Ok(await reader.ReadToEndAsync());
            }
        }

        /// PUT api/values/5
        /// <summary>The put.</summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        #endregion
    }
}