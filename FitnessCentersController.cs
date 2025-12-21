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
    public class FitnessCentersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FitnessCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.FitnessCenter.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var fitnessCenter = await _context.FitnessCenter
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessCenter == null) return NotFound();

            return View(fitnessCenter);
        }

       

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Description,OpeningTime,ClosingTime,PriceMonthly,Price3Months,Price6Months,Price12Months")] FitnessCenter fitnessCenter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fitnessCenter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessCenter);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var fitnessCenter = await _context.FitnessCenter.FindAsync(id);
            if (fitnessCenter == null) return NotFound();
            return View(fitnessCenter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Description,OpeningTime,ClosingTime,PriceMonthly,Price3Months,Price6Months,Price12Months")] FitnessCenter fitnessCenter)
        {
            if (id != fitnessCenter.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fitnessCenter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessCenterExists(fitnessCenter.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fitnessCenter);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var fitnessCenter = await _context.FitnessCenter.FirstOrDefaultAsync(m => m.Id == id);
            if (fitnessCenter == null) return NotFound();
            return View(fitnessCenter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessCenter = await _context.FitnessCenter.FindAsync(id);
            if (fitnessCenter != null) _context.FitnessCenter.Remove(fitnessCenter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FitnessCenterExists(int id)
        {
            return _context.FitnessCenter.Any(e => e.Id == id);
        }
    }
}