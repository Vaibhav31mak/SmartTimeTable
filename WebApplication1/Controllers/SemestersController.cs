using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SemestersController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Semester>>> GetSemesters()
        {
            var userId = GetUserId();
            return await _context.Semesters.Where(s => s.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Semester>> CreateSemester(Semester semester)
        {
            semester.UserId = GetUserId();
            _context.Semesters.Add(semester);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSemesters), new { id = semester.Id }, semester);
        }

        // PUT: api/Semesters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, Semester semester)
        {
            if (id != semester.Id) return BadRequest();

            var userId = GetUserId();
            var existing = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (existing == null) return NotFound();

            existing.Name = semester.Name;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var userId = GetUserId();
            var semester = await _context.Semesters.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (semester == null) return NotFound();

            _context.Semesters.Remove(semester);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}