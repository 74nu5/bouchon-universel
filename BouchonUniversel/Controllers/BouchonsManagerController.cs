namespace BouchonUniversel.Controllers
{
    #region Usings

    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using BouchonUniversel.DAL;
    using BouchonUniversel.Metier;
    using BouchonUniversel.Models.Bouchons;
    using BouchonUniversel.Utils;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    #endregion

    /// <summary>The bouchons controller.</summary>
    public class BouchonsManagerController : Controller
    {
        /// <summary>The _context.</summary>
        private readonly DataContext context;

        private readonly SettingsBouchonMetier settingsBouchon;

        /// <summary>Initializes a new instance of the <see cref="BouchonsManagerController" /> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="settingsBouchon">Application settings.</param>
        public BouchonsManagerController(DataContext context, SettingsBouchonMetier settingsBouchon)
        {
            this.context = context;
            this.settingsBouchon = settingsBouchon;
        }

        /// <summary>GET: Bouchons/Create.</summary>
        /// <returns>The <see cref="IActionResult" />.</returns>
        public IActionResult Create() => this.View();

        /// <summary>
        ///     POST: Bouchons/Create
        ///     To protect from overposting attacks, please enable the specific properties you want to bind to, for
        ///     more details see http://go.microsoft.com/fwlink/?LinkId=317598..
        /// </summary>
        /// <param name="bouchon">The bouchon.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BaseUrl,ServiceUrl,IsEnabled")] Bouchon bouchon)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(bouchon);
            }

            this.context.Add(bouchon);
            await this.context.SaveChangesAsync();
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>GET: Bouchons/Delete/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
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
        /// <returns>The <see cref="Task" />.</returns>
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

        /// <summary>GET: Bouchons/Details/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
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

        [HttpGet("download")]
        public IActionResult DownloadFiles()
        {
            var filesDirectrory = this.settingsBouchon.GetFilesPath();
            var directoryInfo = new DirectoryInfo(filesDirectrory);

            var tempFileName = Path.GetTempFileName();
            ZipUtils.CreateTarGZ(tempFileName, directoryInfo.FullName);
            var fileStream = System.IO.File.OpenRead(tempFileName);

            return this.File(fileStream, "application/gzip", "files.tar.gz");
        }

        /// <summary>GET: Bouchons/Edit/5.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task" />.</returns>
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
        ///     POST: Bouchons/Edit/5
        ///     To protect from overposting attacks, please enable the specific properties you want to bind to, for
        ///     more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="bouchon">The bouchon.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BaseUrl,ServiceUrl,IsEnabled")] Bouchon bouchon)
        {
            if (id != bouchon.BaseUrl)
            {
                return this.NotFound();
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(bouchon);
            }

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

        /// <summary>GET: Bouchons.</summary>
        /// <returns>The <see cref="Task" />.</returns>
        public async Task<IActionResult> Index() => this.View(await this.context.Bouchons.ToListAsync());

        /// <summary>The bouchon exists.</summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="bool" />.</returns>
        private bool BouchonExists(string id) => this.context.Bouchons.Any(e => e.BaseUrl == id);
    }
}
