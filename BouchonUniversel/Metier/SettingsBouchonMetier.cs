namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The bouchon metier.</summary>
    [UsedImplicitly]
    public class SettingsBouchonMetier
    {
        /// <summary>The context.</summary>
        private readonly SettingsBouchonDAO dao;

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonMetier" /> class.</summary>
        /// <param name="dao">The context.</param>
        public SettingsBouchonMetier(SettingsBouchonDAO dao)
            => this.dao = dao;

        /// <summary>The activate bouchon.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> ActivateBouchonAsync()
        {
            try
            {
                return await this.dao.UpdateConfBouchonAsync(true);
            }
            catch (Exception e)
            {
                Debug.Write($"[ERROR] {e.Message}");
                return false;
            }
        }

        /// <summary>The desactivate bouchon.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public async Task<bool> DesactivateBouchonAsync()
        {
            try
            {
                return await this.dao.UpdateConfBouchonAsync(false);
            }
            catch (Exception e)
            {
                Debug.Write($"[ERROR] {e.Message}");
                return false;
            }
        }

        /// <summary>The get bouchon state.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool GetBouchonState()
        {
            try
            {
                return this.dao.GetBouchonState();
            }
            catch (Exception e)
            {
                Debug.Write($"[ERROR] {e.Message}");
                throw;
            }
        }

        /// <summary>The get file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="Stream" />.</returns>
        public Stream GetFile(string fileName)
            => new FileInfo(Path.Combine(this.dao.GetCheminFichier(), fileName)).OpenRead();

        /// <summary>The get file.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public string GetFilesPath()
            => this.dao.GetCheminFichier();

        /// <summary>The get files.</summary>
        /// <returns>The files list.</returns>
        public IEnumerable<string> GetFiles()
        {
            var dirInfo = new DirectoryInfo(this.dao.GetCheminFichier());
            if (!dirInfo.Exists)
            {
                throw new DirectoryNotFoundException($"Le dossier {this.dao.GetCheminFichier()} est inexistant.");
            }

            return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Select(info => info.FullName.Replace($"{this.dao.GetCheminFichier()}\\", string.Empty));
        }
    }
}
