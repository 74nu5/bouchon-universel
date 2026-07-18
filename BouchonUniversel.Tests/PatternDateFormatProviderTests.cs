namespace BouchonUniversel.Tests
{
    using System;
    using System.IO;

    using BouchonUniversel.Services;

    using Xunit;

    /// <summary>Tests unitaires du chargement en cache des patterns de dates (<see cref="PatternDateFormatProvider" />).</summary>
    public class PatternDateFormatProviderTests
    {
        private static string TestConfigPath
            => Path.Combine(AppContext.BaseDirectory, "TestData", "PatternDateFormatConfig.json");

        [Fact]
        public void Config_LoadsPatternsFromFile()
        {
            var provider = new PatternDateFormatProvider(TestConfigPath);

            Assert.NotNull(provider.Config);
            Assert.NotNull(provider.Config.Patterns);
            Assert.Equal(2, provider.Config.Patterns.Count);
            Assert.Equal("yyyy-MM-dd", provider.Config.Patterns[0].Format);
        }

        [Fact]
        public void Config_IsCachedAcrossAccesses()
        {
            var provider = new PatternDateFormatProvider(TestConfigPath);

            // La configuration est chargée une seule fois : la même instance est renvoyée à chaque accès.
            Assert.Same(provider.Config, provider.Config);
        }

        [Fact]
        public void Constructor_MissingFile_Throws()
        {
            var missing = Path.Combine(AppContext.BaseDirectory, "TestData", "inexistant.json");

            Assert.Throws<FileNotFoundException>(() => new PatternDateFormatProvider(missing));
        }
    }
}
