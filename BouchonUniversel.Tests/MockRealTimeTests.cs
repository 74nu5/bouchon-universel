namespace BouchonUniversel.Tests
{
    using System;
    using System.Collections.Generic;

    using BouchonUniversel.Models;
    using BouchonUniversel.Utils;

    using Newtonsoft.Json.Linq;

    using Xunit;

    /// <summary>Tests unitaires de la logique de bouchonnage temps réel (<see cref="MockRealTime" />).</summary>
    public class MockRealTimeTests
    {
        private static readonly IReadOnlyList<PatternDateFormat> Patterns = new[]
        {
            new PatternDateFormat
            {
                Pattern = "(19|20)\\d{2}[/.-](0[1-9]|1[012])[/.-](0[1-9]|[12][0-9]|3[0-9])",
                Format = "yyyy-MM-dd",
            },
        };

        [Fact]
        public void AjustDates_NullPatterns_Throws()
            => Assert.Throws<ArgumentNullException>(() => "peu importe".AjustDates("2020-01-01", null!));

        [Fact]
        public void AjustDates_EmptyPatterns_ReturnsDocumentUnchanged()
        {
            const string doc = "réponse contenant 2020-05-10 sans patterns";

            var result = doc.AjustDates("2021-05-10", Array.Empty<PatternDateFormat>());

            Assert.Equal(doc, result);
        }

        [Fact]
        public void AjustDates_NoMatchingDate_ReturnsDocumentUnchanged()
        {
            const string doc = "aucune date ici, juste du texte";

            var result = doc.AjustDates("2021-05-10", Patterns);

            Assert.Equal(doc, result);
        }

        [Fact]
        public void AjustDates_WithMatchingDate_ReplacesOriginalDate()
        {
            const string doc = "{\"date\":\"2020-05-10\"}";

            var result = doc.AjustDates("2021-05-10", Patterns);

            // La date d'origine doit avoir été remplacée par une date recalculée.
            Assert.DoesNotContain("2020-05-10", result);
            Assert.NotEqual(doc, result);
        }

        [Fact]
        public void ResolveResponse_Json_ReturnsParsedObject()
        {
            var result = "{\"a\":1}".ResolveResponse("application/json");

            Assert.IsType<JObject>(result);
        }

        [Fact]
        public void ResolveResponse_InvalidJson_ReturnsRawString()
        {
            const string invalid = "{ceci n'est pas du json";

            var result = invalid.ResolveResponse("application/json");

            Assert.Equal(invalid, result);
        }

        [Fact]
        public void ResolveResponse_PlainText_ReturnsRawString()
        {
            const string text = "bonjour";

            var result = text.ResolveResponse("text/plain");

            Assert.Equal(text, result);
        }
    }
}
