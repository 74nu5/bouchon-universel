namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System.Net;
    using System.Threading.Tasks;

    using Xunit;

    #endregion

    /// <summary>Tests d'intégration du middleware d'installation (comportement préservé malgré la mise en cache).</summary>
    public class InstallMiddlewareIntegrationTests
    {
        [Fact]
        public async Task NotInstalled_RedirectsToInstall()
        {
            using var factory = new BouchonWebAppFactory(seedData: false);
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            var response = await client.GetAsync("/Home/Index");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/install", response.Headers.Location?.OriginalString ?? string.Empty);
        }

        [Fact]
        public async Task NotInstalled_ApiStaysAccessible()
        {
            using var factory = new BouchonWebAppFactory(seedData: false);
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            // L'API healthcheck ne doit jamais être redirigée vers /install.
            var response = await client.GetAsync("/api/bouchon/healthcheck");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Installed_DoesNotRedirect()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            var response = await client.GetAsync("/Home/Index");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
