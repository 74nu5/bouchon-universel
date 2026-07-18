namespace BouchonUniversel.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Utils;

    using Xunit;

    /// <summary>Tests du relais HTTP des verbes PUT/DELETE/PATCH (<see cref="PassthroughHttp" />).</summary>
    public class PassthroughHttpTests
    {
        [Fact]
        public async Task SendAsync_Put_SendsBodyAndMapsResponse()
        {
            HttpRequestMessage captured = null;
            string capturedBody = null;
            var handler = new StubHandler(async req =>
            {
                captured = req;
                capturedBody = req.Content == null ? null : await req.Content.ReadAsStringAsync();
                var response = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent("{\"ok\":true}") };
                response.Headers.TryAddWithoutValidation("X-Test", "abc");
                return response;
            });

            using var client = new HttpClient(handler);
            var result = await PassthroughHttp.SendAsync(
                client,
                HttpMethod.Put,
                "http://downstream/resource/1",
                new Dictionary<string, IEnumerable<string>>(),
                "{\"name\":\"a\"}",
                new Request());

            Assert.Equal(HttpMethod.Put, captured.Method);
            Assert.Equal("{\"name\":\"a\"}", capturedBody);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("{\"ok\":true}", result.Body);
            Assert.Contains(result.Headers, kv => kv.Key == "X-Test");
        }

        [Fact]
        public async Task SendAsync_Delete_DoesNotSendBody()
        {
            var hadContent = true;
            var handler = new StubHandler(req =>
            {
                hadContent = req.Content != null;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent) { Content = new StringContent(string.Empty) });
            });

            using var client = new HttpClient(handler);
            var result = await PassthroughHttp.SendAsync(
                client,
                HttpMethod.Delete,
                "http://downstream/resource/1",
                new Dictionary<string, IEnumerable<string>>(),
                "ignored body",
                new Request());

            Assert.False(hadContent);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public async Task SendAsync_Patch_UsesPatchVerb()
        {
            HttpMethod method = null;
            var handler = new StubHandler(req =>
            {
                method = req.Method;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("patched") });
            });

            using var client = new HttpClient(handler);
            var result = await PassthroughHttp.SendAsync(
                client,
                HttpMethod.Patch,
                "http://downstream/resource/1",
                new Dictionary<string, IEnumerable<string>>(),
                "{}",
                new Request());

            Assert.Equal("PATCH", method.Method);
            Assert.Equal("patched", result.Body);
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> responder;

            public StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> responder)
                => this.responder = responder;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => this.responder(request);
        }
    }
}
