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

        /// <summary>Initializes a new instance of the <see cref="ServicesController" /> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="metier">The metier.</param>
        public ServicesController(DataContext context, BouchonsMetier metier)
        {
            this.context = context;
            this.metier = metier;
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
            [Bind("Cle,EnvironnementId,Url,Id,IsEnabled,UpdateDates")]
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
            => this.PhysicalFile(name, "application/xml", Path.GetFileName(name));

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
            [Bind("Cle,EnvironnementId,Url,Id,IsEnabled,UpdateDates")]
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
