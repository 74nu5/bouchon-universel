namespace BouchonUniversel.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using BouchonUniversel.Utils;

    using Xunit;

    /// <summary>Test d'aller-retour export (tar.gz) puis import (extraction) d'un jeu de mocks (<see cref="ZipUtils" />).</summary>
    public class ZipUtilsTests
    {
        [Fact]
        public void CreateThenExtract_RoundTripsFileContent()
        {
            var work = Path.Combine(Path.GetTempPath(), "bouchon-zip-" + Guid.NewGuid().ToString("N"));
            var source = Path.Combine(work, "src");
            var dest = Path.Combine(work, "dest");
            var archive = Path.Combine(work, "mocks.tar.gz");
            Directory.CreateDirectory(Path.Combine(source, "cle", "env"));
            Directory.CreateDirectory(dest);
            const string content = "{\"mock\":\"value\"}";
            File.WriteAllText(Path.Combine(source, "cle", "env", "GET_hash.xml"), content);

            try
            {
                ZipUtils.CreateTarGZ(archive, source);

                using (var stream = File.OpenRead(archive))
                {
                    ZipUtils.ExtractTarGZ(stream, dest);
                }

                var extracted = Directory.EnumerateFiles(dest, "GET_hash.xml", SearchOption.AllDirectories).SingleOrDefault();
                Assert.NotNull(extracted);
                Assert.Equal(content, File.ReadAllText(extracted));
            }
            finally
            {
                if (Directory.Exists(work))
                {
                    Directory.Delete(work, true);
                }
            }
        }
    }
}
