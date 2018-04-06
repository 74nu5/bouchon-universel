    using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BouchonUniversel.DAL;
using BouchonUniversel.Models.Bouchons;

namespace BouchonUniversel.Controllers
{
    public class EnvironnementsController : Controller
    {
        private readonly DataContext _context;

        public EnvironnementsController(DataContext context)
        {
            _context = context;
        }

        // GET: Environnements
        public async Task<IActionResult> Index()
        {
            return View(await _context.Environnement.ToListAsync());
        }

        // GET: Environnements/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environnement = await _context.Environnement
                .SingleOrDefaultAsync(m => m.EnvironnementId == id);
            if (environnement == null)
            {
                return NotFound();
            }

            return View(environnement);
        }

        // GET: Environnements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Environnements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnvironnementId,Nom,IsEnabled")] Environnement environnement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(environnement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(environnement);
        }

        // GET: Environnements/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environnement = await _context.Environnement.SingleOrDefaultAsync(m => m.EnvironnementId == id);
            if (environnement == null)
            {
                return NotFound();
            }
            return View(environnement);
        }

        // POST: Environnements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("EnvironnementId,Nom,IsEnabled")] Environnement environnement)
        {
            if (id != environnement.EnvironnementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(environnement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnvironnementExists(environnement.EnvironnementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(environnement);
        }

        // GET: Environnements/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environnement = await _context.Environnement
                .SingleOrDefaultAsync(m => m.EnvironnementId == id);
            if (environnement == null)
            {
                return NotFound();
            }

            return View(environnement);
        }

        // POST: Environnements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var environnement = await _context.Environnement.SingleOrDefaultAsync(m => m.EnvironnementId == id);
            _context.Environnement.Remove(environnement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnvironnementExists(long id)
        {
            return _context.Environnement.Any(e => e.EnvironnementId == id);
        }
    }
}
