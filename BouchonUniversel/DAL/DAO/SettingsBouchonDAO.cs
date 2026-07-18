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
        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonDAO" /> class.</summary>
        /// <param name="context">The context.</param>
        public SettingsBouchonDAO(DataContext context)
            : base(context)
        {
        }

        /// <summary>The get bouchon state.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool GetBouchonState()
            => bool.Parse(this.Querable.FirstOrDefault(bouchon => bouchon.Key == "IsActivated")?.Value);

        /// <summary>The get chemin fichier.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public string GetCheminFichier()
            => this.Querable.FirstOrDefault(bouchon => bouchon.Key == nameof(ApplicationSettings.CheminFichiers))?.Value;

        /// <summary>Insère le paramètre s'il n'existe pas, sinon met à jour sa valeur (upsert par clé).</summary>
        /// <param name="key">La clé du paramètre.</param>
        /// <param name="value">La valeur à enregistrer.</param>
        /// <returns>The <see cref="int" /> (nombre de lignes affectées).</returns>
        public async Task<int> UpsertAsync(string key, string value)
        {
            var existing = this.Querable.FirstOrDefault(setting => setting.Key == key);
            if (existing == null)
            {
                return await this.CreateAsync(new SettingsBouchon { Key = key, Value = value }).ConfigureAwait(false);
            }

            existing.Value = value;
            return await this.UpdateAsync(existing).ConfigureAwait(false);
        }

        /// <summary>The update conf bouchon.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> UpdateConfBouchonAsync(bool isActivated)
        {
            var isActivatedSetting = this.Querable.FirstOrDefault(bouchon => bouchon.Key == "IsActivated");
            if (isActivatedSetting == null)
            {
                return false;
            }

            isActivatedSetting.Value = isActivated.ToString();
            await this.UpdateAsync(isActivatedSetting).ConfigureAwait(false);
            this.SaveChanges();
            return true;
        }

        /// <summary>
        /// Method that determines if there are settings in database.
        /// </summary>
        /// <returns>Returns True if the database is empty, Fasle otherwise.</returns>
        public bool IsSettingsMissing()
            => this.Querable.Count() < 2;
    }
}
