namespace BouchonUniversel.Utils
{
    #region Usings

    using System.IO;
    using System.Text;

    using ICSharpCode.SharpZipLib.GZip;
    using ICSharpCode.SharpZipLib.Tar;

    #endregion

    public static class ZipUtils
    {
        /// <summary>Extrait une archive tar.gz dans un répertoire de destination.</summary>
        /// <param name="gzStream">Flux de l'archive tar.gz.</param>
        /// <param name="destinationDirectory">Répertoire cible.</param>
        /// <remarks>La traversée de répertoire parent est interdite (protection contre le « tar slip »).</remarks>
        public static void ExtractTarGZ(Stream gzStream, string destinationDirectory)
        {
            using var gzipStream = new GZipInputStream(gzStream);
            using var tarArchive = TarArchive.CreateInputTarArchive(gzipStream, Encoding.UTF8);
            tarArchive.ExtractContents(destinationDirectory, false);
        }

        public static void CreateTarGZ(string tgzFilename, string sourceDirectory)
        {
            var outStream = File.Create(tgzFilename);
            var gzoStream = new GZipOutputStream(outStream);
            var tarArchive = TarArchive.CreateOutputTarArchive(gzoStream);

            // Note that the RootPath is currently case sensitive and must be forward slashes e.g. "c:/temp"
            // and must not end with a slash, otherwise cuts off first char of filename
            // This is scheduled for fix in next release
            tarArchive.RootPath = sourceDirectory.Replace('\\', '/');
            if (tarArchive.RootPath.EndsWith("/"))
            {
                tarArchive.RootPath = tarArchive.RootPath.Remove(tarArchive.RootPath.Length - 1);
            }

            AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);

            tarArchive.Close();
        }

        private static void AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
        {
            // Optionally, write an entry for the directory itself.
            // Specify false for recursion here if we will add the directory's files individually.
            var tarEntry = TarEntry.CreateEntryFromFile(sourceDirectory);
            tarArchive.WriteEntry(tarEntry, false);

            // Write each file to the tar.
            var filenames = Directory.GetFiles(sourceDirectory);
            foreach (var filename in filenames)
            {
                tarEntry = TarEntry.CreateEntryFromFile(filename);
                tarArchive.WriteEntry(tarEntry, true);
            }

            if (!recurse)
            {
                return;
            }

            var directories = Directory.GetDirectories(sourceDirectory);
            foreach (var directory in directories)
            {
                AddDirectoryFilesToTar(tarArchive, directory, recurse);
            }
        }
    }
}
