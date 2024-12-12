using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SkiServiceManagement.Models
{
    public class Serviceauftrag
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string KundenName { get; set; }
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Telefon { get; set; }
        [Required]
        [MaxLength(30)]
        public string Prioritaet { get; set; }
        [Required]
        [MaxLength(250)]
        public string Dienstleistung { get; set; }
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime PickupDate { get; set; }
        [Required]
        [MaxLength(100)]
        public string Status { get; set; } = "Offen";
    }


}
