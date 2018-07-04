namespace BouchonUniversel.Models
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    public class PaternDateFormatConfig
    {
        /// <summary>The list of patterns.</summary>
        public List<PaternDateFormat> patterns { get; set; }
    }

    public class PaternDateFormat
    {
        /// <summary>A patern</summary>
        public string pattern { get; set; }

        /// <summary>The format of date</summary>
        public string format { get; set; }
    }
}