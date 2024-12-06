using Microsoft.AspNetCore.Mvc;
using SkiServiceManagement.Data;
using SkiServiceManagement.Models;

namespace SkiServiceManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(Benutzer benutzer)
        {
            _context.Benutzer.Add(benutzer);
            _context.SaveChanges();
            return Ok("Registrierung erfolgreich");
        }
    }
}
