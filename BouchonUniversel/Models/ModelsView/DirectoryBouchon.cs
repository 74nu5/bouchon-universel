namespace BouchonUniversel.Models.ModelsView
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    /// <summary>The dossier bouchon.</summary>
    public sealed class DirectoryBouchon
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the directories.</summary>
        public List<DirectoryBouchon> Directories { get; set; }

        /// <summary>Gets or sets the file bouchons.</summary>
        public List<FileBouchon> FileBouchons { get; set; }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        #endregion
    }
}