namespace BouchonUniversel.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Models;
    using BouchonUniversel.Models.ModelsView;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Mvc;

    [Route("install")]
    public class InstallController : Controller
    {
        private readonly SettingsBouchonDAO settingsBouchonDAO;

        public InstallController(SettingsBouchonDAO settingsBouchonDAO)
        {
            this.settingsBouchonDAO = settingsBouchonDAO;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost("")]
        public async Task<IActionResult> SaveSettings([NotNull] SettingsViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var directoryInfo = new DirectoryInfo(model.FilesPath);
            if (!directoryInfo.Exists)
            {
                this.ModelState.AddModelError("DirectoryDoesNotExists", "Le dossier renseigné n'existe pas.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View("Index", model);
            }

            var defaultActivationSettings = new SettingsBouchon { Key = "DefautActivation", Value = model.DefaultActivation.ToString() };
            var filesPathSettings = new SettingsBouchon { Key = "CheminFichiers", Value = model.FilesPath };

            await this.settingsBouchonDAO.CreateAsync(defaultActivationSettings).ConfigureAwait(false);
            await this.settingsBouchonDAO.CreateAsync(filesPathSettings).ConfigureAwait(false);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
