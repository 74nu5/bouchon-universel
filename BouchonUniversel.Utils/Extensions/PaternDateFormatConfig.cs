namespace BouchonUniversel.Models {
    #region Usings
    using System.Collections.Generic;
    #endregion

    public class PaternDateFormatConfig {
        public List<PaternDateFormat> patterns { get; set; }
    }

    public class PaternDateFormat {
        public string pattern { get; set; }

        public string format { get; set; }
    }
}