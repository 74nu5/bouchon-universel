namespace BouchonUniversel.Models
{
    /// <summary>
    ///     Paramètres d'authentification de l'administration (section « Admin » de la configuration).
    ///     L'authentification n'est activée que si un identifiant et un mot de passe sont renseignés.
    /// </summary>
    public class AdminSettings
    {
        /// <summary>Gets or sets the admin username.</summary>
        public string Username { get; set; }

        /// <summary>Gets or sets the admin password.</summary>
        public string Password { get; set; }

        /// <summary>Gets a value indicating whether l'authentification admin est activée.</summary>
        public bool IsEnabled
            => !string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password);
    }
}
