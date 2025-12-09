using System.Collections.Generic;

namespace GymTrack.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Description { get; set; }

        // Çalışma saatleri 
        public string? OpeningTime { get; set; }
        public string? ClosingTime { get; set; }

        // İlişkiler
        public ICollection<Service>? Services { get; set; }
        public ICollection<Trainer>? Trainers { get; set; }
    }
}
