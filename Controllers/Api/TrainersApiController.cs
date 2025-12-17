using System.Linq;
using System.Threading.Tasks;
using GymTrack.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymTrack.Controllers.Api
{
    [ApiController]
    [Route("api/trainers")] // 🔴 NET ROUTE
    [Authorize(Roles = "Admin")]
    public class TrainersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/trainers
        [HttpGet]
        public async Task<IActionResult> GetAll(
            string? specialty,
            int? fitnessCenterId,
            string? q)
        {
            var query = _context.Trainer
                .AsNoTracking()
                .Include(t => t.FitnessCenter)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(specialty))
                query = query.Where(t => t.Specialty != null && t.Specialty.Contains(specialty));

            if (fitnessCenterId.HasValue)
                query = query.Where(t => t.FitnessCenterId == fitnessCenterId);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t =>
                    (t.FullName != null && t.FullName.Contains(q)) ||
                    (t.Bio != null && t.Bio.Contains(q)));

            var result = await query
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.Bio,
                    FitnessCenter = t.FitnessCenter!.Name
                })
                .ToListAsync();

            return Ok(result);
        }

        // GET: api/trainers/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trainer = await _context.Trainer
                .AsNoTracking()
                .Include(t => t.FitnessCenter)
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.FullName,
                    t.Specialty,
                    t.Bio,
                    FitnessCenter = t.FitnessCenter!.Name
                })
                .FirstOrDefaultAsync();

            if (trainer == null) return NotFound();
            return Ok(trainer);
        }
    }
}
