namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Xunit;

    #endregion

    /// <summary>Tests d'intégration des capacités de plateforme (santé, CORS, HEAD, en-têtes de sécurité, sauvegarde).</summary>
    public class PlatformIntegrationTests
    {
        [Fact]
        public async Task Health_ReturnsHealthy()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            var response = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task SecurityHeaders_ArePresent()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            var response = await client.GetAsync("/api/bouchon/healthcheck");

            Assert.True(response.Headers.Contains("X-Content-Type-Options"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
        }

        [Fact]
        public async Task Cors_ReflectsAllowOriginHeader()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/bouchon/healthcheck");
            request.Headers.Add("Origin", "https://example.com");
            var response = await client.SendAsync(request);

            Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        }

        [Fact]
        public async Task Head_UnknownKey_IsRoutedAndReturns404()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            using var request = new HttpRequestMessage(HttpMethod.Head, "/api/bouchon/inconnu/dev/x");
            var response = await client.SendAsync(request);

            // HEAD est routé (auparavant 405) et remonte le code sémantique 404.
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Backup_ReturnsSqliteDatabaseFile()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient();

            var response = await client.GetAsync("/backup");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Assert.StartsWith("SQLite format 3", Encoding.ASCII.GetString(bytes.Take(15).ToArray()));
        }
    }
}
