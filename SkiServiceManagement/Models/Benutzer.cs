using System.ComponentModel.DataAnnotations;

namespace SkiServiceManagement.Models
{
    public class Benutzer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Benutzername { get; set; }

        [Required]
        public string Passwort { get; set; }
    }
}
