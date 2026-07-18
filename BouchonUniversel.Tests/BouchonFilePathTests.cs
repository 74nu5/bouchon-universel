namespace BouchonUniversel.Tests
{
    using System.IO;

    using BouchonUniversel.Utils;

    using Xunit;

    /// <summary>Tests de la résolution sécurisée de chemin (<see cref="BouchonFilePath.TryResolve" />).</summary>
    public class BouchonFilePathTests
    {
        private static readonly string Root = Path.Combine(Path.GetTempPath(), "bouchons-tests");

        [Fact]
        public void TryResolve_PathInsideRoot_ReturnsTrue()
        {
            var candidate = Path.Combine(Root, "cle", "env", "GET_hash.xml");

            var ok = BouchonFilePath.TryResolve(Root, candidate, out var resolved);

            Assert.True(ok);
            Assert.Equal(Path.GetFullPath(candidate), resolved);
        }

        [Fact]
        public void TryResolve_DirectoryTraversal_ReturnsFalse()
        {
            var candidate = Path.Combine(Root, "..", "secret.txt");

            var ok = BouchonFilePath.TryResolve(Root, candidate, out var resolved);

            Assert.False(ok);
            Assert.Null(resolved);
        }

        [Fact]
        public void TryResolve_SiblingPrefixDirectory_ReturnsFalse()
        {
            // Un répertoire frère dont le nom commence par la racine ne doit pas passer.
            var candidate = Path.Combine(Path.GetTempPath(), "bouchons-tests-evil", "x.txt");

            var ok = BouchonFilePath.TryResolve(Root, candidate, out _);

            Assert.False(ok);
        }

        [Theory]
        [InlineData(null, "x")]
        [InlineData("", "x")]
        [InlineData("root", null)]
        [InlineData("root", "")]
        public void TryResolve_NullOrEmpty_ReturnsFalse(string root, string path)
            => Assert.False(BouchonFilePath.TryResolve(root, path, out _));
    }
}
