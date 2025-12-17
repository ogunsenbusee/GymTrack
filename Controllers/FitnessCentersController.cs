using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymTrack.Data;
using GymTrack.Models;

namespace GymTrack.Controllers
{
    [Authorize(Roles = "Admin")]
    public class FitnessCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FitnessCenters
        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessCenter.ToListAsync());
        }

        // GET: FitnessCenters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var fitnessCenter = await _context.FitnessCenter
                .Include(fc => fc.Trainers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fitnessCenter == null) return NotFound();

            return View(fitnessCenter);
        }

        // GET: FitnessCenters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FitnessCenters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FitnessCenter fitnessCenter)
        {
            if (!ModelState.IsValid)
                return View(fitnessCenter);

            _context.FitnessCenter.Add(fitnessCenter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FitnessCenters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fitnessCenter = await _context.FitnessCenter.FindAsync(id);
            if (fitnessCenter == null) return NotFound();

            return View(fitnessCenter);
        }

        // POST: FitnessCenters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FitnessCenter fitnessCenter)
        {
            if (id != fitnessCenter.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(fitnessCenter);

            var fcFromDb = await _context.FitnessCenter.FindAsync(id);
            if (fcFromDb == null) return NotFound();

            fcFromDb.Name = fitnessCenter.Name;
            fcFromDb.Address = fitnessCenter.Address;
            fcFromDb.Description = fitnessCenter.Description;
            fcFromDb.OpeningTime = fitnessCenter.OpeningTime;
            fcFromDb.ClosingTime = fitnessCenter.ClosingTime;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: FitnessCenters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fitnessCenter = await _context.FitnessCenter
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fitnessCenter == null) return NotFound();

            return View(fitnessCenter);
        }

        // POST: FitnessCenters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCenter = await _context.FitnessCenter.FindAsync(id);
            if (fitnessCenter != null)
            {
                _context.FitnessCenter.Remove(fitnessCenter);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
