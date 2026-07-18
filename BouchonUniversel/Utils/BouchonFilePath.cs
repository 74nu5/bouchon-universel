namespace BouchonUniversel.Utils
{
    #region Usings

    using System;
    using System.IO;

    #endregion

    /// <summary>
    ///     Résolution sécurisée d'un chemin de fichier de bouchon.
    ///     Garantit que le chemin demandé se situe bien à l'intérieur du répertoire racine des bouchons
    ///     (protection contre les attaques par traversée de répertoire).
    /// </summary>
    public static class BouchonFilePath
    {
        /// <summary>Tente de résoudre un chemin en s'assurant qu'il reste sous <paramref name="root" />.</summary>
        /// <param name="root">Répertoire racine autorisé.</param>
        /// <param name="path">Chemin demandé (absolu ou relatif).</param>
        /// <param name="fullPath">Chemin absolu résolu si valide ; sinon <c>null</c>.</param>
        /// <returns><c>true</c> si le chemin est valide et contenu dans la racine.</returns>
        public static bool TryResolve(string root, string path, out string fullPath)
        {
            fullPath = null;

            if (string.IsNullOrWhiteSpace(root) || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            string rootFull;
            string candidate;
            try
            {
                rootFull = Path.GetFullPath(root);
                candidate = Path.GetFullPath(path);
            }
            catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
            {
                return false;
            }

            var rootWithSeparator = rootFull.EndsWith(Path.DirectorySeparatorChar)
                ? rootFull
                : rootFull + Path.DirectorySeparatorChar;

            if (!candidate.StartsWith(rootWithSeparator, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            fullPath = candidate;
            return true;
        }
    }
}
