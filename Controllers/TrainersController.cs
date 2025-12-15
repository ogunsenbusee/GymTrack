using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTrack.Data;
using GymTrack.Models;

namespace GymTrack.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var trainers = _context.Trainer
                .Include(t => t.FitnessCenter);

            return View(await trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            ViewData["FitnessCenterId"] = new SelectList(
                _context.FitnessCenter.AsNoTracking().OrderBy(fc => fc.Name),
                "Id",
                "Name"
            );
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,Bio,FitnessCenterId")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["FitnessCenterId"] = new SelectList(
                _context.FitnessCenter.AsNoTracking().OrderBy(fc => fc.Name),
                "Id",
                "Name",
                trainer.FitnessCenterId
            );
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer == null) return NotFound();

            ViewData["FitnessCenterId"] = new SelectList(
                _context.FitnessCenter.AsNoTracking().OrderBy(fc => fc.Name),
                "Id",
                "Name",
                trainer.FitnessCenterId
            );
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty,Bio,FitnessCenterId")] Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["FitnessCenterId"] = new SelectList(
                    _context.FitnessCenter.AsNoTracking().OrderBy(fc => fc.Name),
                    "Id",
                    "Name",
                    trainer.FitnessCenterId
                );
                return View(trainer);
            }

            var trainerFromDb = await _context.Trainer.FirstOrDefaultAsync(t => t.Id == id);
            if (trainerFromDb == null) return NotFound();

            // Güvenli güncelleme (Update(trainer) yerine)
            trainerFromDb.FullName = trainer.FullName;
            trainerFromDb.Specialty = trainer.Specialty;
            trainerFromDb.Bio = trainer.Bio;
            trainerFromDb.FitnessCenterId = trainer.FitnessCenterId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExists(trainer.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainer.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainer.Any(e => e.Id == id);
        }
    }
}
