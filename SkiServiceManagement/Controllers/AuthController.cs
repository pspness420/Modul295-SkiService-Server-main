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
using System.Security.Cryptography;
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
        public IActionResult Login(LoginRequest request)
        {
            // Benutzer validieren...
            var user = _context.Benutzer.SingleOrDefault(u => u.Benutzername == request.Username);
            if (user == null || user.Passwort != request.Password)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Benutzername),
                new Claim(ClaimTypes.Role, user.Rolle)
            };

            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // RefreshToken in der Datenbank speichern
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7); // RefreshToken ist 7 Tage gültig
            _context.SaveChanges();

            return Ok(new
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] string refreshToken)
        {
            // Hier den Refresh-Token aus der Datenbank oder dem Cache entfernen
            // RemoveRefreshTokenFromDatabase(refreshToken);

            return Ok(new { message = "Erfolgreich abgemeldet." });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh(TokenRefreshRequest request)
        {
            var user = _context.Benutzer.SingleOrDefault(u => u.RefreshToken == request.RefreshToken);

            if (user == null || user.RefreshTokenExpiry <= DateTime.Now)
            {
                return Unauthorized("Ungültiger oder abgelaufener RefreshToken.");
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                return Unauthorized("Ungültiges AccessToken.");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);
            _context.SaveChanges();

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
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