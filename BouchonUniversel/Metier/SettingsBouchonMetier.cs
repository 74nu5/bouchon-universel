namespace BouchonUniversel.Metier
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using BouchonUniversel.DAL.DAO;

    using JetBrains.Annotations;

    #endregion

    /// <summary>The bouchon metier.</summary>
    [UsedImplicitly]
    public class SettingsBouchonMetier
    {
        #region Champs

        /// <summary>The context.</summary>
        private readonly SettingsBouchonDAO dao;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="SettingsBouchonMetier"/> class.</summary>
        /// <param name="dao">The context.</param>
        public SettingsBouchonMetier(SettingsBouchonDAO dao) => this.dao = dao;

        #endregion

        #region Méthodes publiques

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

        /// <summary>The activate bouchon.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool ActivateBouchon()
        {
            try
            {
                return this.dao.UpdateConfBouchon(true);
            }
            catch (Exception e)
            {
                Debug.Write($"[ERROR] {e.Message}");
                return false;
            }
        }

        /// <summary>The desactivate bouchon.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        public bool DesactivateBouchon()
        {
            try
            {
                return this.dao.UpdateConfBouchon(false);
            }
            catch (Exception e)
            {
                Debug.Write($"[ERROR] {e.Message}");
                return false;
            }
        }

        /// <summary>The get files.</summary>
        /// <returns>The files list.</returns>
        public IEnumerable<string> GetFiles()
        {
            var dirInfo = new DirectoryInfo(this.dao.GetCheminFichier());
            return dirInfo.EnumerateFiles("*", SearchOption.AllDirectories).Select(info => info.FullName.Replace(this.dao.GetCheminFichier() + "\\", string.Empty));
        }

        /// <summary>The get file.</summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The <see cref="Stream"/>.</returns>
        public Stream GetFile(string fileName) => new FileInfo(Path.Combine(this.dao.GetCheminFichier(), fileName)).OpenRead();

        #endregion
    }
}