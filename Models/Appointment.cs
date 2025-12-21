using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GymTrack.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        
        public string? UserId { get; set; }

        
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public string? Note { get; set; }

        public string Status { get; set; } = "Beklemede";
    }
}