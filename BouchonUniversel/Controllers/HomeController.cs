namespace BouchonUniversel.Controllers
{
    #region Usings

    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL.DAO;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Models;
    using BouchonUniversel.Models.ModelsView;

    using Microsoft.AspNetCore.Mvc;

    #endregion

    /// <summary>The home controller.</summary>
    public sealed class HomeController : Controller
    {
        /// <summary>The metier.</summary>
        private readonly SettingsBouchonMetier metier;

        private readonly ServicesDAO servicesDAO;

        private readonly EnvironnementDAO environnementDAO;

        /// <summary>Initializes a new instance of the <see cref="HomeController" /> class.</summary>
        /// <param name="metier">The metier.</param>
        /// <param name="servicesDAO">The services DAO.</param>
        /// <param name="environnementDAO">The environnement DAO.</param>
        public HomeController(SettingsBouchonMetier metier, ServicesDAO servicesDAO, EnvironnementDAO environnementDAO)
        {
            this.metier = metier;
            this.servicesDAO = servicesDAO;
            this.environnementDAO = environnementDAO;
        }

        /// <summary>The error.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Error()
            => this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });

        /// <summary>The get file.</summary>
        /// <param name="fileSelected">The file selected.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult GetFile(string fileSelected)
            => this.File(this.metier.GetFile(fileSelected), "application/octet-stream", fileSelected);

        /// <summary>Tableau de bord d'accueil.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public async Task<IActionResult> Index()
            => this.View(await this.BuildDashboardAsync().ConfigureAwait(false));

        /// <summary>Active ou désactive le bouchonnage global, puis réaffiche le tableau de bord.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        [HttpPost]
        public async Task<IActionResult> Index(bool isActivated)
        {
            var result = isActivated ? await this.metier.ActivateBouchonAsync().ConfigureAwait(false) : await this.metier.DesactivateBouchonAsync().ConfigureAwait(false);
            this.TempData["Message"] = result
                ? (isActivated ? "Bouchonnage global activé." : "Bouchonnage global désactivé.")
                : "Erreur lors de la mise à jour du bouchonnage.";

            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>Construit le modèle du tableau de bord de manière résiliente.</summary>
        /// <returns>The <see cref="DashboardViewModel" />.</returns>
        private async Task<DashboardViewModel> BuildDashboardAsync()
        {
            return new DashboardViewModel
                   {
                       ServicesCount = (await this.servicesDAO.GetAllAsync().ConfigureAwait(false)).Count,
                       EnvironnementsCount = (await this.environnementDAO.GetAllAsync().ConfigureAwait(false)).Count,
                       MockFilesCount = SafeCount(() => this.metier.GetFiles().Count()),
                       IsBouchonActivated = SafeState(() => this.metier.GetBouchonState()),
                       FilesPath = this.metier.GetFilesPath(),
                   };
        }

        private static int SafeCount(Func<int> count)
        {
            try
            {
                return count();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static bool? SafeState(Func<bool> state)
        {
            try
            {
                return state();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
