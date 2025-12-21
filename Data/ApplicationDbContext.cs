using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GymTrack.Models;

namespace GymTrack.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<Trainer> Trainer { get; set; }
        public DbSet<FitnessCenter> FitnessCenter { get; set; }
        public DbSet<Service> Service { get; set; }
    }
}