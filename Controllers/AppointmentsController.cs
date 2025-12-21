using GymTrack.Data;
using GymTrack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymTrack.Controllers
{
    [Authorize] 
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null) return RedirectToAction("Login", "Account");

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Where(a => a.UserId == user.Id) 
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .OrderByDescending(a => a.Date) 
                .ToListAsync();

            return View(appointments);
        }

        
        public IActionResult Create()
        {
            
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            var user = await _userManager.GetUserAsync(User);

            appointment.UserId = user.Id;
            appointment.Status = "Beklemede";

            
            bool isBusy = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date);

            if (isBusy)
            {
                ModelState.AddModelError("", "Bu saatte eğitmen dolu.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyAppointments));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "Id", "FullName", appointment.TrainerId);
            return View(appointment);
       
        
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "Onaylandı";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage));
        }

       
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = "İptal Edildi";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage));
        }

        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage));
        }
    }

}