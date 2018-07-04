namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.Linq;
    using System.Threading.Tasks;

    using DAL;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    using Models.Bouchons;

    #endregion

    /// <summary>The environnements controller.</summary>
    public class EnvironnementsController : Controller
    {
        #region Champs

        /// <summary>The context.</summary>
        private readonly DataContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="EnvironnementsController" /> class.</summary>
        /// <param name="context">The context.</param>
        public EnvironnementsController(DataContext context)
            => this.context = context;

        #endregion

        #region Méthodes publiques

        /// <summary>The create.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Create()
            => this.View();

        /// <summary>The create.</summary>
        /// <param name="environnement">The environnement.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,IsEnabled")] Environnement environnement)
        {
            if (this.ModelState.IsValid)
            {
                this.context.Add(environnement);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(environnement);
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

            var environnement = await this.context.Environnement.SingleOrDefaultAsync(m => m.Id == id);
            if (environnement == null)
            {
                return this.NotFound();
            }

            return this.View(environnement);
        }

        /// <summary>The delete confirmed.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var environnement = await this.context.Environnement.SingleOrDefaultAsync(m => m.Id == id);
            this.context.Environnement.Remove(environnement);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>The details.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var environnement = await this.context.Environnement.SingleOrDefaultAsync(env => env.Id == id);
            if (environnement == null)
            {
                return this.NotFound();
            }

            return this.View(environnement);
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

            var environnement = await this.context.Environnement.SingleOrDefaultAsync(m => m.Id == id);
            if (environnement == null)
            {
                return this.NotFound();
            }

            return this.View(environnement);
        }

        /// <summary>The edit.</summary>
        /// <param name="id">The id.</param>
        /// <param name="environnement">The environnement.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Nom,IsEnabled")] Environnement environnement)
        {
            if (id != environnement.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.context.Update(environnement);
                    await this.context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.EnvironnementExists(environnement.Id))
                    {
                        return this.NotFound();
                    }

                    throw;
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(environnement);
        }

        /// <summary>The index.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Index()
            => this.View(await this.context.Environnement.ToListAsync());

        #endregion

        #region Méthodes privées

        /// <summary>The environnement exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private bool EnvironnementExists(long id)
            => this.context.Environnement.Any(e => e.Id == id);

        #endregion
    }
}