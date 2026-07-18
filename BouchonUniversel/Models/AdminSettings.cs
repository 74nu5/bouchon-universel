namespace BouchonUniversel.Models
{
    /// <summary>
    ///     Paramètres d'authentification de l'administration (section « Admin » de la configuration).
    ///     L'authentification n'est activée que si un identifiant est renseigné, avec soit un hash de mot de passe
    ///     (<see cref="PasswordHash" />, recommandé), soit un mot de passe en clair (<see cref="Password" />, dépannage/dev).
    /// </summary>
    public class AdminSettings
    {
        /// <summary>Gets or sets the admin username.</summary>
        public string Username { get; set; }

        /// <summary>Gets or sets the admin password hash (format PasswordHasher ASP.NET Core, PBKDF2 salé). Chemin recommandé.</summary>
        public string PasswordHash { get; set; }

        /// <summary>Gets or sets the admin password en clair. Déconseillé (dépannage / dev) : préférer <see cref="PasswordHash" />.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets l'identifiant du compte lecteur (rôle lecture seule), facultatif.</summary>
        public string ViewerUsername { get; set; }

        /// <summary>Gets or sets le hash du mot de passe lecteur (recommandé).</summary>
        public string ViewerPasswordHash { get; set; }

        /// <summary>Gets or sets le mot de passe lecteur en clair (dépannage / dev).</summary>
        public string ViewerPassword { get; set; }

        /// <summary>Gets a value indicating whether l'authentification admin est activée.</summary>
        public bool IsEnabled
            => !string.IsNullOrEmpty(this.Username)
               && (!string.IsNullOrEmpty(this.PasswordHash) || !string.IsNullOrEmpty(this.Password));
    }
}
