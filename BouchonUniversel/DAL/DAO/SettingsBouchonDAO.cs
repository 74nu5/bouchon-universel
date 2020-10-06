namespace BouchonUniversel.DAL.DAO
{
    #region Usings

    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.Models;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The memory context dao.</summary>
    [UsedImplicitly]
    public class SettingsBouchonDAO : BaseDAO<DataContext, SettingsBouchon, long>
    {
        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonDAO"/> class.</summary>
        /// <param name="context">The context.</param>
        public SettingsBouchonDAO(DataContext context)
            : base(context)
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>The get bouchon state.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool GetBouchonState()
            => bool.Parse(this.Querable.FirstOrDefault(bouchon => bouchon.Key == "IsActivated")?.Value);

        /// <summary>The get chemin fichier.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public string GetCheminFichier()
            => this.Querable.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.CheminFichiers))?.Value;

        /// <summary>The update conf bouchon.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public async Task<bool> UpdateConfBouchonAsync(bool isActivated)
        {
            var isActivatedSetting = this.Querable.FirstOrDefault(bouchon => bouchon.Key == "IsActivated");
            if (isActivatedSetting == null)
            {
                return false;
            }

            isActivatedSetting.Value = isActivated.ToString();
            await this.UpdateAsync(isActivatedSetting);
            this.SaveChanges();
            return true;
        }

        #endregion
    }
}