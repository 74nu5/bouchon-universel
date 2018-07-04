namespace BouchonUniversel.Models
{
    #region Usings

    using System.Collections.Generic;

    #endregion

    public class PatternDateFormatConfig
    {
        public List<PatternDateFormat> patterns { get; set; }
    }

    public class PatternDateFormat
    {
        public string pattern { get; set; }

        public string format { get; set; }
    }
}