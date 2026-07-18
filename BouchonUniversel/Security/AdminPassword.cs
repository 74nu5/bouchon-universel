namespace BouchonUniversel.Security
{
    #region Usings

    using System;
    using System.Security.Cryptography;
    using System.Text;

    using BouchonUniversel.Models;

    using Microsoft.AspNetCore.Identity;

    #endregion

    /// <summary>
    ///     Hachage et vérification du mot de passe administrateur.
    ///     Utilise <see cref="PasswordHasher{TUser}" /> (PBKDF2 salé) pour le chemin recommandé ; conserve une
    ///     comparaison à temps constant pour le mot de passe en clair (dépannage / dev).
    /// </summary>
    public static class AdminPassword
    {
        private static readonly PasswordHasher<object> Hasher = new ();

        /// <summary>Produit un hash auto-porteur (algorithme, sel, itérations) pour un mot de passe.</summary>
        /// <param name="password">Le mot de passe en clair.</param>
        /// <returns>Le hash à stocker dans <c>Admin:PasswordHash</c>.</returns>
        public static string Hash(string password)
            => Hasher.HashPassword(null, password ?? string.Empty);

        /// <summary>Vérifie un couple identifiant / mot de passe pour le compte administrateur.</summary>
        /// <param name="settings">Les paramètres d'administration.</param>
        /// <param name="username">L'identifiant saisi.</param>
        /// <param name="password">Le mot de passe saisi.</param>
        /// <returns><c>true</c> si les identifiants administrateur sont valides.</returns>
        public static bool Verify(AdminSettings settings, string username, string password)
            => settings != null && Matches(username, password, settings.Username, settings.PasswordHash, settings.Password);

        /// <summary>Résout le rôle correspondant aux identifiants (administrateur puis lecteur), ou <c>null</c> si invalide.</summary>
        /// <param name="settings">Les paramètres d'administration.</param>
        /// <param name="username">L'identifiant saisi.</param>
        /// <param name="password">Le mot de passe saisi.</param>
        /// <returns>Le rôle (<see cref="Roles.Admin" /> / <see cref="Roles.Viewer" />) ou <c>null</c>.</returns>
        public static string ResolveRole(AdminSettings settings, string username, string password)
        {
            if (settings == null)
            {
                return null;
            }

            if (Matches(username, password, settings.Username, settings.PasswordHash, settings.Password))
            {
                return Roles.Admin;
            }

            if (Matches(username, password, settings.ViewerUsername, settings.ViewerPasswordHash, settings.ViewerPassword))
            {
                return Roles.Viewer;
            }

            return null;
        }

        private static bool Matches(string username, string password, string expectedUsername, string hash, string plaintext)
        {
            if (string.IsNullOrEmpty(expectedUsername) || !string.Equals(username, expectedUsername, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(hash))
            {
                return Hasher.VerifyHashedPassword(null, hash, password ?? string.Empty) != PasswordVerificationResult.Failed;
            }

            if (!string.IsNullOrEmpty(plaintext))
            {
                // Repli sur le mot de passe en clair (déconseillé) : comparaison à temps constant.
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(password ?? string.Empty),
                    Encoding.UTF8.GetBytes(plaintext));
            }

            return false;
        }
    }
}
