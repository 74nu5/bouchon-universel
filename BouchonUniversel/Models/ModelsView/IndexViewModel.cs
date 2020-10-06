namespace BouchonUniversel.Models.ModelsView
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    /// <summary>The index view model.</summary>
    public class IndexViewModel
    {
        /// <summary>Gets or sets the files.</summary>
        public IEnumerable<string> Files { get; set; } = new List<string>();

        /// <summary>Gets or sets a value indicating whether is bouchon activated.</summary>
        public bool? IsBouchonActivated { get; set; }
    }
}
