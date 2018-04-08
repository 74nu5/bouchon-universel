namespace BouchonUniversel.Models
{
    #region Usings

    using System.ComponentModel.DataAnnotations.Schema;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The settings bouchon.</summary>
    [UsedImplicitly]
    public class SettingsBouchon : IDto<long>
    {
        #region Propriétés et indexeurs

        /// <summary>Gets or sets the id.</summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>Gets or sets the key.</summary>
        public string Key { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public string Value { get; set; }

        #endregion
    }
}