namespace BouchonUniversel.Models.ModelsView
{
    /// <summary>Modèle de vue pour l'édition du contenu d'une réponse bouchonnée (fichier sur disque).</summary>
    public sealed class EditFileViewModel
    {
        /// <summary>Gets or sets the full path of the mocked response file.</summary>
        public string Path { get; set; }

        /// <summary>Gets or sets the display file name.</summary>
        public string FileName { get; set; }

        /// <summary>Gets or sets the raw file content.</summary>
        public string Content { get; set; }
    }
}
