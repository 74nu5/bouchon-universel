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
    }
}
