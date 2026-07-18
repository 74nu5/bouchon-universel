namespace BouchonUniversel.Tests
{
    using BouchonUniversel.Utils;

    using Xunit;

    /// <summary>Tests des décisions d'ingénierie du chaos (<see cref="ChaosPlan" />).</summary>
    public class ChaosPlanTests
    {
        [Theory]
        [InlineData(0, 0, false)]   // probabilité nulle -> jamais
        [InlineData(0, 99, false)]
        [InlineData(100, 99, true)] // probabilité maximale -> toujours
        [InlineData(50, 49, true)]  // tirage sous le seuil -> injection
        [InlineData(50, 50, false)] // tirage au seuil -> pas d'injection
        [InlineData(50, 51, false)]
        public void ShouldInjectError_RespectsProbabilityAndRoll(int probability, int roll, bool expected)
            => Assert.Equal(expected, ChaosPlan.ShouldInjectError(probability, roll));

        [Theory]
        [InlineData(0, 500)]     // non renseigné -> 500 par défaut
        [InlineData(503, 503)]
        [InlineData(418, 418)]
        public void ResolveErrorStatusCode_DefaultsTo500(int configured, int expected)
            => Assert.Equal(expected, ChaosPlan.ResolveErrorStatusCode(configured));

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(100, 99, true)]
        [InlineData(30, 29, true)]
        [InlineData(30, 30, false)]
        public void ShouldResetConnection_RespectsProbabilityAndRoll(int probability, int roll, bool expected)
            => Assert.Equal(expected, ChaosPlan.ShouldResetConnection(probability, roll));

        [Theory]
        [InlineData(0, 0, false)]
        [InlineData(100, 0, true)]
        [InlineData(50, 49, true)]
        [InlineData(50, 50, false)]
        public void ShouldTruncate_RespectsProbabilityAndRoll(int probability, int roll, bool expected)
            => Assert.Equal(expected, ChaosPlan.ShouldTruncate(probability, roll));

        [Theory]
        [InlineData("123456", "123")]
        [InlineData("a", "a")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Truncate_ReturnsFirstHalf(string input, string expected)
            => Assert.Equal(expected, ChaosPlan.Truncate(input));
    }
}
