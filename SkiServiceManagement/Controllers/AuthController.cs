using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SkiServiceManagement.Models;
using SkiServiceManagement.Data;
using System;

namespace SkiServiceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Benutzer benutzer)
        {
            if (await _context.Benutzer.AnyAsync(u => u.Benutzername == benutzer.Benutzername))
                return BadRequest("Benutzername existiert bereits.");

            benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(benutzer.Passwort); // Passwort verschlüsseln
            _context.Benutzer.Add(benutzer);
            await _context.SaveChangesAsync();
            return Ok("Benutzer erfolgreich registriert.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var benutzer = await _context.Benutzer.SingleOrDefaultAsync(u => u.Benutzername == loginRequest.Benutzername);
            if (benutzer == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Passwort, benutzer.Passwort))
                return Unauthorized("Ungültige Anmeldedaten.");

            var token = GenerateJwtToken(benutzer);
            return Ok(new { Token = token });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBenutzer(int id)
        {
            var userRole = User.FindFirst("role")?.Value;
            if (userRole != "Admin")
                return Forbid("Nur Admins können Benutzer löschen.");

            var benutzer = await _context.Benutzer.FindAsync(id);
            if (benutzer == null)
                return NotFound();

            _context.Benutzer.Remove(benutzer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBenutzer(int id, Benutzer updatedBenutzer)
        {
            var userRole = User.FindFirst("role")?.Value;
            if (userRole != "Admin")
                return Forbid("Nur Admins können Benutzer bearbeiten.");

            var benutzer = await _context.Benutzer.FindAsync(id);
            if (benutzer == null)
                return NotFound();

            benutzer.Benutzername = updatedBenutzer.Benutzername;
            benutzer.Passwort = BCrypt.Net.BCrypt.HashPassword(updatedBenutzer.Passwort);
            benutzer.Rolle = updatedBenutzer.Rolle;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private string GenerateJwtToken(Benutzer benutzer)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, benutzer.Benutzername),
                new Claim("role", benutzer.Rolle),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey")); // Passen Sie den Schlüssel an
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
