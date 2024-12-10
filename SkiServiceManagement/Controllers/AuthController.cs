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
                u.Rolle
            }).ToList();
            return Ok(users);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Benutzer benutzer)
        {
            // Benutzername aus Vorname und Nachname zusammensetzen
            if (string.IsNullOrWhiteSpace(benutzer.Benutzername))
            {
                if (!string.IsNullOrWhiteSpace(benutzer.Vorname) && !string.IsNullOrWhiteSpace(benutzer.Nachname))
                {
                    benutzer.Benutzername = $"{benutzer.Vorname} {benutzer.Nachname}";
                }
                else
                {
                    return BadRequest("Vorname und Nachname sind erforderlich, um den Benutzernamen zu erstellen.");
                }
            }

            // Überprüfung auf Duplikate
            if (await _context.Benutzer.AnyAsync(u => u.Benutzername == benutzer.Benutzername))
                return BadRequest("Ein Benutzer mit diesem Namen existiert bereits.");

            if (await _context.Benutzer.AnyAsync(u => u.Email == benutzer.Email))
                return BadRequest("Ein Benutzer mit dieser E-Mail existiert bereits.");

            // Passwort hashen
            benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(benutzer.Passwort, workFactor: 10);

            _context.Benutzer.Add(benutzer);
            await _context.SaveChangesAsync();

            return Ok("Benutzer erfolgreich registriert.");
        }

        // Login per Benutzername oder Email
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            // Benutzer suchen anhand von Benutzername oder E-Mail
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
        
        [HttpGet("admin-endpoint")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("Nur Admins können diesen Endpunkt aufrufen.");
        }

        [HttpGet("mitarbeiter-endpoint")]
        [Authorize(Policy = "MitarbeiterOnly")]
        public IActionResult MitarbeiterOnlyEndpoint()
        {
            return Ok("Mitarbeiter und Admins können diesen Endpunkt aufrufen.");
        }
    }
}