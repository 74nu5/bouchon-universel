namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Xunit;

    #endregion

    /// <summary>Tests d'intégration bout-en-bout de l'API de bouchonnage (pipeline complet via WebApplicationFactory).</summary>
    public class BouchonApiIntegrationTests
    {
        [Fact]
        public async Task Healthcheck_Returns200Ok()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            var response = await client.GetAsync("/api/bouchon/healthcheck");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("OK", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task UnknownKey_Returns404WithSemanticCode()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            var response = await client.GetAsync("/api/bouchon/inconnu/dev/ressource");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Contains("1001", await response.Content.ReadAsStringAsync());
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("PATCH")]
        [InlineData("DELETE")]
        public async Task AllVerbs_UnknownKey_Return404(string verb)
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            using var request = new HttpRequestMessage(new HttpMethod(verb), "/api/bouchon/inconnu/dev/x");
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ChaosService_InjectsConfiguredErrorStatus()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            // Le service « chaos » est seedé avec ErrorProbability=100 et ErrorStatusCode=503.
            var response = await client.GetAsync("/api/bouchon/chaos/dev/quelconque");

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}
