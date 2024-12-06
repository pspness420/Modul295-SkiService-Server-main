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
        public string KundenName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Telefon { get; set; }
        [Required]
        public string Prioritaet { get; set; }
        [Required]
        public string Dienstleistung { get; set; }
        public string Status { get; set; } = "Offen";
    }
}
