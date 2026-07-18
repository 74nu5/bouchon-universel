namespace BouchonUniversel.Models.ModelsView
{
    /// <summary>Modèle du tableau de bord d'accueil.</summary>
    public sealed class DashboardViewModel
    {
        /// <summary>Gets or sets the number of services.</summary>
        public int ServicesCount { get; set; }

        /// <summary>Gets or sets the number of environnements.</summary>
        public int EnvironnementsCount { get; set; }

        /// <summary>Gets or sets the number of mock response files.</summary>
        public int MockFilesCount { get; set; }

        /// <summary>Gets or sets a value indicating whether le bouchonnage global est activé.</summary>
        public bool? IsBouchonActivated { get; set; }

        /// <summary>Gets or sets the mock files root path.</summary>
        public string FilesPath { get; set; }
    }
}
