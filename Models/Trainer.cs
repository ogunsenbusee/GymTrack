namespace GymTrack.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? Bio { get; set; }

        public int FitnessCenterId { get; set; }
        public FitnessCenter? FitnessCenter { get; set; }
    }
}
