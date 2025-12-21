using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymTrack.Data;
using GymTrack.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymTrack.Controllers
{
    
    [Authorize]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Trainer.Include(t => t.FitnessCenter);
            return View(await applicationDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trainer == null) return NotFound();

            return View(trainer);
        }

        

        
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenter, "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,FullName,Specialty,Bio,FitnessCenterId")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenter, "Id", "Name", trainer.FitnessCenterId);
            return View(trainer);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer == null) return NotFound();

            ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenter, "Id", "Name", trainer.FitnessCenterId);
            return View(trainer);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Specialty,Bio,FitnessCenterId")] Trainer trainer)
        {
            if (id != trainer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FitnessCenterId"] = new SelectList(_context.FitnessCenter, "Id", "Name", trainer.FitnessCenterId);
            return View(trainer);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer
                .Include(t => t.FitnessCenter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainer.Remove(trainer);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainer.Any(e => e.Id == id);
        }
    }
}