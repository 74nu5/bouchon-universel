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
            var model = new SettingsViewModel();

            // Pré-remplissage avec la configuration existante pour permettre une reconfiguration.
            if (!this.settingsBouchonDAO.IsSettingsMissing())
            {
                model.FilesPath = this.settingsBouchonDAO.GetCheminFichier();
                try
                {
                    model.DefaultActivation = this.settingsBouchonDAO.GetBouchonState();
                }
                catch (System.Exception)
                {
                    // Paramètre « IsActivated » absent ou invalide : on laisse la valeur par défaut.
                }
            }

            return this.View(model);
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

            // Upsert par clé : évite les doublons si l'installation est re-soumise ou si les paramètres ont déjà été amorcés.
            // Les clés correspondent à celles lues par l'application (cf. SettingsBouchonDAO / DbInitializer).
            // « UrlService » n'est pas géré ici : il est amorcé au démarrage et hors du périmètre de ce formulaire.
            await this.settingsBouchonDAO.UpsertAsync("IsActivated", model.DefaultActivation.ToString()).ConfigureAwait(false);
            await this.settingsBouchonDAO.UpsertAsync(nameof(ApplicationSettings.CheminFichiers), model.FilesPath).ConfigureAwait(false);

            return this.RedirectToAction("Index", "Home");
        }
    }
}
