namespace BouchonUniversel.Models.Bouchons
{
    /// <summary>The response erreur.</summary>
    public class ResponseErreur
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the message.</summary>
        public string Message { private get; set; }

        /// <summary>Gets or sets the code.</summary>
        public int Code { private get; set; }

        /// <summary>The code message.</summary>
        public object CodeMessage
            => $"{this.Code} - {this.Message}";

        #endregion
    }
}