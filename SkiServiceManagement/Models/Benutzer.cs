using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SkiServiceManagement.Models
{
    public class Benutzer
    {   
        public ObjectId Id { get; set; }

        public string Vorname { get; set; }

        public string Nachname { get; set; }
        
        public string Benutzername { get; set; }

        public string Email { get; set; }

        public string Passwort { get; set; }

        public string Rolle { get; set; } = "Kunde"; // Standardrolle ist Kunde

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
