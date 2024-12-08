using System.ComponentModel.DataAnnotations;

namespace SkiServiceManagement.Models
{
    public class Benutzer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Benutzername { get; set; }

        [Required]
        [MaxLength(100)]
        public string Passwort { get; set; }

        [Required]
        [MaxLength(20)]
        public string Rolle { get; set; } = "Kunde"; // Standardrolle ist Kunde
    }
}
