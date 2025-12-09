using System;

namespace GymTrack.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsApproved { get; set; } = false;
        public decimal Price { get; set; }

        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        // AspNetUsers tablosundaki kullanıcının Id'si
        public string UserId { get; set; } = string.Empty;
    }
}
