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

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonDAO"/> class.</summary>
        /// <param name="context">The context.</param>
        public SettingsBouchonDAO(DataContext context) : base(context)
        {
        }

        #endregion

        public bool GetBouchonState() => Convert.ToBoolean(this.Entities.FirstOrDefault(bouchon => bouchon.Key == "IsActivated")?.Value);

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

        public string GetCheminFichier() => this.Entities.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.CheminFichiers))?.Value;
    }
}