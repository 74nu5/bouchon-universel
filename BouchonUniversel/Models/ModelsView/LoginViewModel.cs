namespace BouchonUniversel.Models.ModelsView
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>Modèle de vue du formulaire de connexion administrateur.</summary>
    public sealed class LoginViewModel
    {
        /// <summary>Gets or sets the username.</summary>
        [Required]
        [Display(Name = "Identifiant")]
        public string Username { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }
    }
}
