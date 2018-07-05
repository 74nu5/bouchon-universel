namespace BouchonUniversel.Models
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    /// <summary>The pattern date format config.</summary>
    public class PatternDateFormatConfig
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the patterns.</summary>
        public List<PatternDateFormat> Patterns { get; set; }

        #endregion
    }
}