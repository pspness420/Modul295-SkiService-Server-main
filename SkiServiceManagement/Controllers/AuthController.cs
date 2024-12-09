using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Threading.Tasks;
using SkiServiceManagement.Models;
using SkiServiceManagement.Data;
using System;
using System.Linq;

namespace SkiServiceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Admin-Endpunkt: Alle Benutzer abrufen
        [HttpGet("users")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Benutzer.Select(u => new
            {
                u.Id,
                u.Benutzername,
                u.Email,
                u.Vorname,
                u.Nachname,
                u.Rolle
            }).ToList();
            return Ok(users);
        }

        // Benutzer registrieren
        [HttpPost("register")]
        public async Task<IActionResult> Register(Benutzer benutzer, string firstName, string lastName)
        {
            // Benutzername aus Vorname und Nachname zusammensetzen
            benutzer.Benutzername = $"{firstName.Trim()} {lastName.Trim()}";

            // Prüfen, ob Benutzername bereits existiert
            if (await _context.Benutzer.AnyAsync(u => u.Benutzername == benutzer.Benutzername))
                return BadRequest("Benutzername existiert bereits.");

            benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(benutzer.Passwort, workFactor: 10); // Passwort hashen

            _context.Benutzer.Add(benutzer);
            await _context.SaveChangesAsync();
            return Ok("Benutzer erfolgreich registriert.");
        }


        // Login per Benutzername oder Email
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var benutzer = await _context.Benutzer
                .SingleOrDefaultAsync(u =>
                    u.Benutzername == loginRequest.Benutzername || u.Email == loginRequest.Benutzername);

            if (benutzer == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Passwort, benutzer.Passwort))
                return Unauthorized("Ungültige Anmeldedaten.");

            var token = GenerateJwtToken(benutzer);
            return Ok(new { Token = token });
        }

        // Benutzer aktualisieren (nur Admin)
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateBenutzer(int id, Benutzer updatedBenutzer)
        {
            var benutzer = await _context.Benutzer.FindAsync(id);
            if (benutzer == null)
                return NotFound();

            benutzer.Benutzername = updatedBenutzer.Benutzername;
            benutzer.Email = updatedBenutzer.Email;
            benutzer.Vorname = updatedBenutzer.Vorname;
            benutzer.Nachname = updatedBenutzer.Nachname;
            benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(updatedBenutzer.Passwort);
            benutzer.Rolle = updatedBenutzer.Rolle;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Benutzer löschen (nur Admin)
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteBenutzer(int id)
        {
            var benutzer = await _context.Benutzer.FindAsync(id);
            if (benutzer == null)
                return NotFound();

            _context.Benutzer.Remove(benutzer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // JWT-Token generieren
        private string GenerateJwtToken(Benutzer benutzer)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, benutzer.Benutzername),
                new Claim("role", benutzer.Rolle),
                new Claim(JwtRegisteredClaimNames.Email, benutzer.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // Wenn man mal nur etwas für mittarbeiter benötigt
        /* 
        [Authorize(Policy = "MitarbeiterOnly")]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok("Nur Mitarbeiter können diesen Endpunkt aufrufen.");
        }
        */
    }
}