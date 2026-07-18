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

        [Fact]
        public async Task Login_IsRateLimited_AfterTooManyAttempts()
        {
            using var factory = new BouchonWebAppFactory("admin", AdminPassword.Hash("s3cret"));
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);

            // La limite est de 5 requêtes par minute ; la 6e doit être rejetée en 429 (avant même la validation).
            System.Net.Http.HttpResponseMessage last = null;
            for (var i = 0; i < 6; i++)
            {
                using var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "/account/login");
                last?.Dispose();
                last = await client.SendAsync(request);
            }

            Assert.Equal((HttpStatusCode)429, last.StatusCode);
            last.Dispose();
        }
    }
}
