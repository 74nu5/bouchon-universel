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

        /// <summary>Vérifie un couple identifiant / mot de passe contre la configuration admin.</summary>
        /// <param name="settings">Les paramètres d'administration.</param>
        /// <param name="username">L'identifiant saisi.</param>
        /// <param name="password">Le mot de passe saisi.</param>
        /// <returns><c>true</c> si les identifiants sont valides.</returns>
        public static bool Verify(AdminSettings settings, string username, string password)
        {
            if (settings == null || !settings.IsEnabled)
            {
                return false;
            }

            if (!string.Equals(username, settings.Username, StringComparison.Ordinal))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(settings.PasswordHash))
            {
                return Hasher.VerifyHashedPassword(null, settings.PasswordHash, password ?? string.Empty) != PasswordVerificationResult.Failed;
            }

            // Repli sur le mot de passe en clair (déconseillé) : comparaison à temps constant.
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(password ?? string.Empty),
                Encoding.UTF8.GetBytes(settings.Password ?? string.Empty));
        }
    }
}
