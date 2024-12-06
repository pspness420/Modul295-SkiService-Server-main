using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SkiServiceManagement.Models;
using SkiServiceManagement.Data;

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

            _context.Benutzer.Add(benutzer);
            await _context.SaveChangesAsync();
            return Ok("Benutzer erfolgreich registriert.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var benutzer = await _context.Benutzer.SingleOrDefaultAsync(u => u.Benutzername == request.Benutzername);
            if (benutzer == null || benutzer.Passwort != request.Passwort)
                return Unauthorized("Ungültige Anmeldedaten.");

            return Ok("Erfolgreich angemeldet.");
        }
    }
}
