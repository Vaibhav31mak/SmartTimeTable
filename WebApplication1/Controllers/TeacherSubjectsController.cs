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
    public class TeacherSubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeacherSubjectsController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherSubject>>> GetLinks()
        {
            var userId = GetUserId();
            return await _context.TeacherSubjects
                .Where(ts => ts.UserId == userId)
                .Include(ts => ts.Teacher)
                .Include(ts => ts.Subject)
                .ToListAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLink(int id)
        {
            var userId = GetUserId();
            var link = await _context.TeacherSubjects.FirstOrDefaultAsync(ts => ts.Id == id && ts.UserId == userId);

            if (link == null) return NotFound();

            _context.TeacherSubjects.Remove(link);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}