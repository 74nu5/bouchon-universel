namespace BouchonUniversel.Services
{
    using System.IO;

    using BouchonUniversel.DAL.DAO;

    public class FileService
    {
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        /// <summary>Initializes a new instance of the <see cref="FileService"/> class.</summary>
        /// <param name="settingsBouchonDAO"></param>
        public FileService(SettingsBouchonDAO settingsBouchonDAO)
            => this.settingsBouchonDAO = settingsBouchonDAO;

        public DirectoryInfo CreateBouchonDirectory(string cle, string env, string route)
        {
            var bouchonDir = new DirectoryInfo(Path.Combine(this.settingsBouchonDAO.GetCheminFichier(), cle, env, route));
            if (!bouchonDir.Exists)
            {
                bouchonDir.Create();
            }

            return bouchonDir;
        }
    }
}
