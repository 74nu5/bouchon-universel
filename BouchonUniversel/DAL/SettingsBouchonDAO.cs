namespace BouchonUniversel.DAL
{
    #region Usings

    using System;
    using System.Linq;

    using JetBrains.Annotations;

    using Models;

    #endregion

    /// <summary>The memory context dao.</summary>
    [UsedImplicitly]
    public class SettingsBouchonDAO
    {
        #region Champs

        /// <summary>The context.</summary>
        private readonly DataContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonDAO"/> class. Initializes a new instance of the<see cref="T:System.Object"></see> class.</summary>
        /// <param name="context">The context.</param>
        public SettingsBouchonDAO(DataContext context) => this.context = context;

        #endregion

        #region Méthodes publiques

        /// <summary>The update conf bouchon.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool UpdateConfBouchon(bool isActivated)
        {
            var isActivatedSetting = this.context.SettingsBouchon.FirstOrDefault(bouchon => bouchon.Key == "IsActivated");
            if (isActivatedSetting == null)
            {
                return false;
            }

            isActivatedSetting.Value = isActivated.ToString();
            this.context.SettingsBouchon.Update(isActivatedSetting);
            this.context.SaveChanges();
            return true;
        }

        /// <summary>The get bouchon state.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool GetBouchonState()
        {
            var isActivatedSetting = this.context.SettingsBouchon.FirstOrDefault(bouchon => bouchon.Key == "IsActivated");
            return Convert.ToBoolean(isActivatedSetting?.Value);
        }

        /// <summary>The get chemin fichier.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public string GetCheminFichier()
        {
            var isActivatedSetting = this.context.SettingsBouchon.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.CheminFichiers));
            return isActivatedSetting?.Value;
        }

        /// <summary>The get url service.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public string GetUrlService()
        {
            var isActivatedSetting = this.context.SettingsBouchon.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.UrlService));
            return isActivatedSetting?.Value;
        }

        #endregion
    }
}