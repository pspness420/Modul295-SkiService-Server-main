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
        [MaxLength(100)]
        public string Status { get; set; } = "Offen";
    }
}
