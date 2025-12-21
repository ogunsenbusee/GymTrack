using System.ComponentModel.DataAnnotations;

namespace GymTrack.Models
{
    public class FitnessCenter
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [Display(Name = "Salon Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres zorunludur.")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Açılış Saati")]
        public string? OpeningTime { get; set; } 

        [Display(Name = "Kapanış Saati")]
        public string? ClosingTime { get; set; } 

        
        [Display(Name = "Aylık Fiyat (TL)")]
        public decimal PriceMonthly { get; set; }

        [Display(Name = "3 Aylık Fiyat (TL)")]
        public decimal Price3Months { get; set; }

        [Display(Name = "6 Aylık Fiyat (TL)")]
        public decimal Price6Months { get; set; }

        [Display(Name = "12 Aylık Fiyat (TL)")]
        public decimal Price12Months { get; set; }
    }
}