namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Models;

    #endregion

    /// <summary>The memory context dao.</summary>
    [UsedImplicitly]
    public class SettingsBouchonDAO : BaseDAO<DataContext, SettingsBouchon, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonDAO" /> class.</summary>
        /// <param name="context">The context.</param>
        public SettingsBouchonDAO(DataContext context)
            : base(context)
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The get bouchon state.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool GetBouchonState() => Convert.ToBoolean(this.Entities.FirstOrDefault(bouchon => bouchon.Key == "IsActivated")?.Value);

        /// <summary>The get chemin fichier.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public string GetCheminFichier() => this.Entities.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.CheminFichiers))?.Value;

        /// <summary>The update conf bouchon.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public bool UpdateConfBouchon(bool isActivated)
        {
            var isActivatedSetting = this.Entities.FirstOrDefault(bouchon => bouchon.Key == "IsActivated");
            if (isActivatedSetting == null)
            {
                return false;
            }

            isActivatedSetting.Value = isActivated.ToString();
            this.Entities.Update(isActivatedSetting);
            this.SaveChanges();
            return true;
        }

        #endregion
    }
}