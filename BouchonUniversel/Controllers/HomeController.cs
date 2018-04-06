namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.Diagnostics;

    using Metier;

    using Microsoft.AspNetCore.Mvc;

    using Models;
    using Models.ModelsView;

    #endregion

    /// <summary>The home controller.</summary>
    public class HomeController : Controller
    {
        #region Champs

        /// <summary>The metier.</summary>
        private readonly SettingsBouchonMetier metier;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="HomeController"/> class.</summary>
        /// <param name="metier">The metier.</param>
        public HomeController(SettingsBouchonMetier metier) => this.metier = metier;

        #endregion

        #region Méthodes publiques

        /// <summary>The index.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Index()
        {
            var model = new IndexViewModel { IsBouchonActivated = this.metier.GetBouchonState(), Files = this.metier.GetFiles() };
            return this.View(model);
        }

        /// <summary>The index.</summary>
        /// <param name="isActivated">The is activated.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        public IActionResult Index(bool isActivated)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = isActivated ? this.metier.ActivateBouchon() : this.metier.DesactivateBouchon();

            if (!result)
            {
                this.ModelState.AddModelError("IsActivated", "Erreur lors de la gestion du bouchon");
                return this.StatusCode(500);
            }

            var model = new IndexViewModel { IsBouchonActivated = isActivated, Files = this.metier.GetFiles() };
            return this.View(model);
        }

        /// <summary>The error.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Error() => this.View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier
        });

        /// <summary>The get file.</summary>
        /// <param name="fileSelected">The file selected.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult GetFile(string fileSelected) => this.File(this.metier.GetFile(fileSelected), "application/octet-stream", fileSelected);

        #endregion
    }
}