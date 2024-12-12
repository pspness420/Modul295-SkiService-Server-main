using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SkiServiceManagement.Models;
using SkiServiceManagement.Data;
using System;

namespace SkiServiceManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuftragController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuftragController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuftraege()
        {
            var auftraege = await _context.Serviceauftraege.ToListAsync();
            return Ok(auftraege);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuftragById(int id)
        {
            var auftrag = await _context.Serviceauftraege.FindAsync(id);
            if (auftrag == null)
                return NotFound();

            return Ok(auftrag);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAuftrag(Serviceauftrag auftrag)
        {
            if (string.IsNullOrWhiteSpace(auftrag.KundenName) ||
                string.IsNullOrWhiteSpace(auftrag.Email) ||
                string.IsNullOrWhiteSpace(auftrag.Dienstleistung))
            {
                return BadRequest(new { message = "Bitte füllen Sie alle erforderlichen Felder aus." });
            }

            try
            {
                auftrag.CreateDate = DateTime.Now;
                auftrag.PickupDate = CalculatePickupDate(auftrag.Prioritaet);

                _context.Serviceauftraege.Add(auftrag);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAuftragById), new { id = auftrag.Id }, auftrag);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Erstellen eines Auftrags: {ex.Message}");
                return StatusCode(500, new { message = "Interner Serverfehler.", error = ex.Message });
            }
        }

        private DateTime CalculatePickupDate(string priority)
        {
            int daysToAdd = priority switch
            {
                "Tief" => 12,
                "Standard" => 7,
                "Express" => 5,
                _ => 7
            };

            DateTime pickupDate = DateTime.Now.AddDays(daysToAdd);

            // Wochenenden überspringen
            while (pickupDate.DayOfWeek == DayOfWeek.Saturday || pickupDate.DayOfWeek == DayOfWeek.Sunday)
            {
                pickupDate = pickupDate.AddDays(1);
            }

            return pickupDate;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuftrag(int id, Serviceauftrag updatedAuftrag)
        {
            var auftrag = await _context.Serviceauftraege.FindAsync(id);
            if (auftrag == null)
                return NotFound();

            auftrag.KundenName = updatedAuftrag.KundenName;
            auftrag.Email = updatedAuftrag.Email;
            auftrag.Telefon = updatedAuftrag.Telefon;
            auftrag.Prioritaet = updatedAuftrag.Prioritaet;
            auftrag.Dienstleistung = updatedAuftrag.Dienstleistung;
            auftrag.Status = updatedAuftrag.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuftrag(int id)
        {
            var auftrag = await _context.Serviceauftraege.FindAsync(id);
            if (auftrag == null)
                return NotFound();

            _context.Serviceauftraege.Remove(auftrag);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}