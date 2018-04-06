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

    /// <summary>The bouchons controller.</summary>
    public class BouchonsManagerController : Controller
    {
        #region Champs

        /// <summary>The _context.</summary>
        private readonly DataContext context;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>Initializes a new instance of the <see cref="BouchonsManagerController"/> class.</summary>
        /// <param name="context">The context.</param>
        public BouchonsManagerController(DataContext context) => this.context = context;

        #endregion

        #region Méthodes publiques

        /// <summary>GET: Bouchons.</summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IActionResult> Index() => this.View(await this.context.Bouchons.ToListAsync());

        /// <summary>GET: Bouchons/Details/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bouchon = await this.context.Bouchons.SingleOrDefaultAsync(m => m.BaseUrl == id);
            if (bouchon == null)
            {
                return this.NotFound();
            }

            return this.View(bouchon);
        }

        /// <summary>GET: Bouchons/Create.</summary>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        public IActionResult Create() => this.View();

        /// <summary>POST: Bouchons/Create
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598..</summary>
        /// <param name="bouchon">The bouchon.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaseUrl,ServiceUrl,IsEnabled")] Bouchon bouchon)
        {
            if (this.ModelState.IsValid)
            {
                this.context.Add(bouchon);
                await this.context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(bouchon);
        }

        /// <summary>GET: Bouchons/Edit/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bouchon = await this.context.Bouchons.SingleOrDefaultAsync(m => m.BaseUrl == id);
            if (bouchon == null)
            {
                return this.NotFound();
            }

            return this.View(bouchon);
        }

        /// <summary>
        /// POST: Bouchons/Edit/5
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="bouchon">The bouchon.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BaseUrl,ServiceUrl,IsEnabled")] Bouchon bouchon)
        {
            if (id != bouchon.BaseUrl)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this.context.Update(bouchon);
                    await this.context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.BouchonExists(bouchon.BaseUrl))
                    {
                        return this.NotFound();
                    }

                    throw;
                }

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(bouchon);
        }

        /// <summary>GET: Bouchons/Delete/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var bouchon = await this.context.Bouchons.SingleOrDefaultAsync(m => m.BaseUrl == id);
            if (bouchon == null)
            {
                return this.NotFound();
            }

            return this.View(bouchon);
        }

        /// <summary>POST: Bouchons/Delete/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bouchon = await this.context.Bouchons.SingleOrDefaultAsync(m => m.BaseUrl == id);
            this.context.Bouchons.Remove(bouchon);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        #endregion

        #region Méthodes privées

        /// <summary>The bouchon exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool BouchonExists(string id) => this.context.Bouchons.Any(e => e.BaseUrl == id);

        #endregion
    }
}