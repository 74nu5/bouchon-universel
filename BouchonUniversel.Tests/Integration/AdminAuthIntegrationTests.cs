namespace BouchonUniversel.Tests.Integration
{
    #region Usings

    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
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

        [Fact]
        public async Task Admin_PassesWriteAuthorization()
        {
            using var factory = new BouchonWebAppFactory("admin", AdminPassword.Hash("adminpwd"));
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);
            await LoginAsync(client, "admin", "adminpwd");

            var read = await client.GetAsync("/Services");
            Assert.Equal(HttpStatusCode.OK, read.StatusCode);

            // Le rôle Admin passe le filtre d'autorisation ; sans jeton anti-forgery, l'anti-forgery rejette ensuite (400).
            var write = await client.PostAsync("/Services/Create", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.BadRequest, write.StatusCode);
        }

        [Fact]
        public async Task Viewer_CanRead_ButCannotWrite()
        {
            using var factory = new BouchonWebAppFactory(
                "admin",
                AdminPassword.Hash("adminpwd"),
                viewerUsername: "lecteur",
                viewerPasswordHash: AdminPassword.Hash("viewpwd"));
            using var client = factory.CreateHttpsClient(allowAutoRedirect: false);
            await LoginAsync(client, "lecteur", "viewpwd");

            // Lecture autorisée.
            var read = await client.GetAsync("/Services");
            Assert.Equal(HttpStatusCode.OK, read.StatusCode);

            // Écriture refusée par le filtre de rôle (avant l'anti-forgery) : redirection d'accès refusé.
            var write = await client.PostAsync("/Services/Create", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.Redirect, write.StatusCode);
            Assert.Contains("/account/login", write.Headers.Location?.OriginalString ?? string.Empty);
        }

        private static async Task LoginAsync(HttpClient client, string username, string password)
        {
            var page = await client.GetAsync("/account/login");
            var html = await page.Content.ReadAsStringAsync();
            var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;

            var form = new Dictionary<string, string>
                       {
                           ["Username"] = username,
                           ["Password"] = password,
                           ["__RequestVerificationToken"] = token,
                       };
            using var response = await client.PostAsync("/account/login", new FormUrlEncodedContent(form));
        }
    }
}
