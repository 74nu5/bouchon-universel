namespace BouchonUniversel.Tests
{
    using System.Collections.Generic;

    using BouchonUniversel.Utils;

    using Xunit;

    /// <summary>Tests de la substitution de jetons dans les réponses (<see cref="ResponseTemplate" />).</summary>
    public class ResponseTemplateTests
    {
        private static readonly IReadOnlyDictionary<string, string> Query = new Dictionary<string, string> { ["id"] = "42" };
        private static readonly IReadOnlyDictionary<string, string> Headers = new Dictionary<string, string> { ["X-Trace"] = "abc" };

        [Fact]
        public void Apply_SubstitutesQueryHeaderAndRoute()
        {
            var result = ResponseTemplate.Apply("{{route}}|{{query.id}}|{{header.X-Trace}}", "clients/1", Query, Headers);

            Assert.Equal("clients/1|42|abc", result);
        }

        [Fact]
        public void Apply_UnknownToken_IsLeftUnchanged()
        {
            var result = ResponseTemplate.Apply("{{query.absent}}-{{inconnu}}", "r", Query, Headers);

            Assert.Equal("-{{inconnu}}", result);
        }

        [Fact]
        public void Apply_GuidAndNow_AreReplaced()
        {
            var result = ResponseTemplate.Apply("{{guid}} {{now}}", "r", Query, Headers);

            Assert.DoesNotContain("{{guid}}", result);
            Assert.DoesNotContain("{{now}}", result);
        }

        [Fact]
        public void Apply_EmptyBody_ReturnsAsIs()
            => Assert.Equal(string.Empty, ResponseTemplate.Apply(string.Empty, "r", Query, Headers));
    }
}
