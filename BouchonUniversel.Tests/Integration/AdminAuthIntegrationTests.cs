namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System.Net;
    using System.Threading.Tasks;

    using BouchonUniversel.Security;

    using Xunit;

    #endregion

    /// <summary>Tests d'intégration de l'authentification admin (pipeline complet).</summary>
    public class AdminAuthIntegrationTests
    {
        [Fact]
        public async Task AuthDisabled_AdminPagesAreAccessible()
        {
            using var factory = new BouchonWebAppFactory();
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            var response = await client.GetAsync("/Services");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AuthEnabled_AdminPageRedirectsToLogin_ButApiStaysOpen()
        {
            using var factory = new BouchonWebAppFactory("admin", AdminPassword.Hash("s3cret"));
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            var admin = await client.GetAsync("/Services");
            Assert.Equal(HttpStatusCode.Redirect, admin.StatusCode);
            Assert.Contains("/account/login", admin.Headers.Location?.OriginalString ?? string.Empty);

            var api = await client.GetAsync("/api/bouchon/healthcheck");
            Assert.Equal(HttpStatusCode.OK, api.StatusCode);
        }

        [Fact]
        public async Task AuthEnabled_LoginPageIsAccessible()
        {
            using var factory = new BouchonWebAppFactory("admin", AdminPassword.Hash("s3cret"));
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            var response = await client.GetAsync("/account/login");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
