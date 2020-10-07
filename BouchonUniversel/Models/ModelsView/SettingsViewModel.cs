namespace BouchonUniversel.Models.ModelsView
{
    using System.ComponentModel.DataAnnotations;

    public class SettingsViewModel
    {
        [Required]
        [Display(Name = "Activation des bouchons par défaut.")]
        public bool DefaultActivation { get; set; }

        [Required]
        [Display(Name = "Chemin des fichiers de bouchons.")]
        public string FilesPath { get; set; }
    }
}
