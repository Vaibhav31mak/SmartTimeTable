using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TimeSlotsController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimeSlot>>> GetTimeSlots()
        {
            var userId = GetUserId();
            return await _context.TimeSlots.Where(t => t.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<TimeSlot>> CreateTimeSlot([FromBody] TimeSlotCreateDto dto)
        {
            // Parse Strings to TimeSpan
            if (!TimeSpan.TryParse(dto.StartTime, out var start) || !TimeSpan.TryParse(dto.EndTime, out var end))
            {
                return BadRequest("Invalid Time Format. Use HH:mm:ss");
            }

            var timeSlot = new TimeSlot
            {
                UserId = GetUserId(),
                StartTime = start,
                EndTime = end,
                IsLunchBreak = dto.IsLunchBreak
            };

            _context.TimeSlots.Add(timeSlot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTimeSlots), new { id = timeSlot.Id }, timeSlot);
        }
        // PUT: api/TimeSlots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] TimeSlotCreateDto dto)
        {
            var userId = GetUserId();
            var existing = await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (existing == null) return NotFound();

            // 1. Parse Strings to TimeSpan (Reuse logic)
            if (!TimeSpan.TryParse(dto.StartTime, out var start) || !TimeSpan.TryParse(dto.EndTime, out var end))
            {
                return BadRequest("Invalid Time Format. Use HH:mm:ss");
            }

            // 2. Update Fields
            existing.StartTime = start;
            existing.EndTime = end;
            existing.IsLunchBreak = dto.IsLunchBreak;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSlot(int id)
        {
            var userId = GetUserId();
            var slot = await _context.TimeSlots.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (slot == null) return NotFound();

            _context.TimeSlots.Remove(slot);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}