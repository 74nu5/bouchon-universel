namespace BouchonUniversel.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Models;
    using BouchonUniversel.Models.ModelsView;

    using JetBrains.Annotations;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
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

            // Les clés doivent correspondre à celles lues par l'application (cf. SettingsBouchonDAO / DbInitializer) :
            // l'activation est stockée sous « IsActivated », et « UrlService » est amorcé pour rester cohérent.
            var isActivatedSettings = new SettingsBouchon { Key = "IsActivated", Value = model.DefaultActivation.ToString() };
            var filesPathSettings = new SettingsBouchon { Key = nameof(ApplicationSettings.CheminFichiers), Value = model.FilesPath };
            var urlServiceSettings = new SettingsBouchon { Key = nameof(ApplicationSettings.UrlService), Value = string.Empty };

            await this.settingsBouchonDAO.CreateAsync(isActivatedSettings).ConfigureAwait(false);
            await this.settingsBouchonDAO.CreateAsync(filesPathSettings).ConfigureAwait(false);
            await this.settingsBouchonDAO.CreateAsync(urlServiceSettings).ConfigureAwait(false);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
