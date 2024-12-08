using System.ComponentModel.DataAnnotations;

namespace SkiServiceManagement.Models
{
    public class LoginRequest
    {
        [Required]
        public string Benutzername { get; set; }

        [Required]
        public string Passwort { get; set; }
    }
}
