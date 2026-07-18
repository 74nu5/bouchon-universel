namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Models.ModelsView;
    using BouchonUniversel.Utils;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    #endregion

    /// <summary>The services controller.</summary>
    public sealed class ServicesController : Controller
    {
        /// <summary>The context.</summary>
        private readonly DataContext context;

        /// <summary>The metier.</summary>
        private readonly BouchonsMetier metier;

        /// <summary>The settings bouchon metier (fournit la racine des fichiers de bouchons).</summary>
        private readonly SettingsBouchonMetier settingsBouchon;

        /// <summary>Initializes a new instance of the <see cref="ServicesController" /> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="metier">The metier.</param>
        /// <param name="settingsBouchon">The settings bouchon metier.</param>
        public ServicesController(DataContext context, BouchonsMetier metier, SettingsBouchonMetier settingsBouchon)
        {
            this.context = context;
            this.metier = metier;
            this.settingsBouchon = settingsBouchon;
        }

        /// <summary>The create.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Create()
        {
            this.ViewData["EnvironnementId"] = new SelectList(this.context.Environnement, nameof(Environnement.Id), nameof(Environnement.Nom));
            return this.View();
        }

        /// <summary>The create.</summary>
        /// <param name="service">The service.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Cle,EnvironnementId,Url,Id,IsEnabled,UpdateDates,LatencyMs,ErrorProbability,ErrorStatusCode")]
            Service service)
        {
            if (this.ModelState.IsValid)
            {
                this.context.Add(service);
                await this.context.SaveChangesAsync().ConfigureAwait(false);
                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["EnvironnementId"] = new SelectList(this.context.Environnement, nameof(Environnement.Id), nameof(Environnement.Nom), service.EnvironnementId);
            return this.View(service);
        }

        /// <summary>The delete.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var service = await this.context.Services.Include(s => s.Environnement).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (service == null)
            {
                return this.NotFound();
            }

            return this.View(service);
        }

        /// <summary>The delete confirmed.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var service = await this.context.Services.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            this.context.Services.Remove(service);
            await this.context.SaveChangesAsync().ConfigureAwait(false);
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>The details.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return this.NotFound("Id null.");
            }

            var model = new DetailsServiceViewModel { Service = await this.context.Services.Include(s => s.Environnement).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false) };
            model.ListeFichiers = this.metier.GetFilesOfService(model.Service);

            if (model.Service == null)
            {
                return this.NotFound("Service non présent en base.");
            }

            return this.View(model);
        }

        /// <summary>The download file.</summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult DownloadFile(string name)
        {
            if (!BouchonFilePath.TryResolve(this.settingsBouchon.GetFilesPath(), name, out var fullPath) || !System.IO.File.Exists(fullPath))
            {
                return this.NotFound("Fichier introuvable ou hors du répertoire des bouchons.");
            }

            return this.PhysicalFile(fullPath, "application/xml", Path.GetFileName(fullPath));
        }

        /// <summary>Affiche le formulaire d'édition du contenu d'une réponse bouchonnée.</summary>
        /// <param name="path">Chemin du fichier de réponse à éditer.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        [HttpGet]
        public async Task<IActionResult> EditFile(string path)
        {
            if (!BouchonFilePath.TryResolve(this.settingsBouchon.GetFilesPath(), path, out var fullPath) || !System.IO.File.Exists(fullPath))
            {
                return this.NotFound("Fichier introuvable ou hors du répertoire des bouchons.");
            }

            var model = new EditFileViewModel
                        {
                            Path = fullPath,
                            FileName = Path.GetFileName(fullPath),
                            Content = await System.IO.File.ReadAllTextAsync(fullPath).ConfigureAwait(false),
                        };

            return this.View(model);
        }

        /// <summary>Enregistre le contenu édité d'une réponse bouchonnée.</summary>
        /// <param name="model">Le modèle de vue contenant le chemin et le nouveau contenu.</param>
        /// <returns>The <see cref="IActionResult" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFile(EditFileViewModel model)
        {
            if (model == null || !BouchonFilePath.TryResolve(this.settingsBouchon.GetFilesPath(), model.Path, out var fullPath) || !System.IO.File.Exists(fullPath))
            {
                return this.NotFound("Fichier introuvable ou hors du répertoire des bouchons.");
            }

            await System.IO.File.WriteAllTextAsync(fullPath, model.Content ?? string.Empty).ConfigureAwait(false);
            this.TempData["Message"] = "Fichier enregistré.";
            return this.RedirectToAction(nameof(this.EditFile), new { path = fullPath });
        }

        /// <summary>The edit.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var service = await this.context.Services.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (service == null)
            {
                return this.NotFound();
            }

            this.ViewData["EnvironnementId"] = new SelectList(this.context.Environnement, nameof(Environnement.Id), nameof(Environnement.Nom), service.EnvironnementId);
            return this.View(service);
        }

        /// <summary>The edit.</summary>
        /// <param name="id">The id.</param>
        /// <param name="service">The service.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            long id,
            [Bind("Cle,EnvironnementId,Url,Id,IsEnabled,UpdateDates,LatencyMs,ErrorProbability,ErrorStatusCode")]
            Service service)
        {
            if (id != service.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.context.Update(service);
                    await this.context.SaveChangesAsync().ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.ServiceExists(service.Id))
                    {
                        return this.NotFound();
                    }

                    throw;
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            this.ViewData["EnvironnementId"] = new SelectList(this.context.Environnement, nameof(Environnement.Id), nameof(Environnement.Nom), service.EnvironnementId);
            return this.View(service);
        }

        /// <summary>The index.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Index()
        {
            var dataContext = this.context.Services.Include(s => s.Environnement);
            return this.View(await dataContext.ToListAsync().ConfigureAwait(false));
        }

        /// <summary>The service exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private bool ServiceExists(long id)
            => this.context.Services.Any(e => e.Id == id);
    }
}
