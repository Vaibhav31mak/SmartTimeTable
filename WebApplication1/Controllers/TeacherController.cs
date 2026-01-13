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
    public class TeachersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TeachersController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // 1. UPDATE GET: Include the links so frontend knows what is checked
        [HttpGet]
        // GET: api/Teachers
        [HttpGet]
        public async Task<ActionResult> GetTeachers() // Changed return type to ActionResult
        {
            var userId = GetUserId();

            // 1. Fetch Teachers
            var teachers = await _context.Teachers
                .Where(t => t.UserId == userId)
                .Include(t => t.Department)
                .ToListAsync();

            // 2. Fetch ALL Links separately (Since we can't use Include)
            var allLinks = await _context.TeacherSubjects
                .Where(ts => ts.UserId == userId)
                .ToListAsync();

            // 3. Manually combine them into a JSON result
            // We create an anonymous object that LOOKS like the model the frontend expects
            var result = teachers.Select(t => new
            {
                t.Id,
                t.Name,
                t.DepartmentId,
                t.Department,
                t.UserId,
                // Manually attach the links here
                TeacherSubjects = allLinks.Where(link => link.TeacherId == t.Id).ToList()
            });

            return Ok(result);
        }

        // PUT: api/Teachers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] TeacherCreateDto dto)
        {
            var userId = GetUserId();

            // 1. Get the Teacher (No Include needed here since we handle links manually)
            var existing = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (existing == null) return NotFound();

            // 2. Update Basic Info
            existing.Name = dto.Name;
            existing.DepartmentId = dto.DepartmentId;

            // 3. Manually Find and Delete Old Links
            var oldLinks = await _context.TeacherSubjects
                .Where(ts => ts.TeacherId == id && ts.UserId == userId)
                .ToListAsync();

            _context.TeacherSubjects.RemoveRange(oldLinks);

            // 4. Add New Links
            if (dto.SubjectIds != null)
            {
                foreach (var subId in dto.SubjectIds)
                {
                    _context.TeacherSubjects.Add(new TeacherSubject
                    {
                        UserId = userId,
                        TeacherId = existing.Id,
                        SubjectId = subId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Teachers (Custom Logic for Checkboxes)
        [HttpPost]
        public async Task<ActionResult<Teacher>> CreateTeacher([FromBody] TeacherCreateDto dto)
        {
            var userId = GetUserId();

            // 1. Create the Teacher
            var teacher = new Teacher
            {
                UserId = userId,
                Name = dto.Name,
                DepartmentId = dto.DepartmentId
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync(); // Generates teacher.Id

            // 2. Loop through checked Subject IDs and create links
            if (dto.SubjectIds != null && dto.SubjectIds.Any())
            {
                foreach (var subjectId in dto.SubjectIds)
                {
                    var link = new TeacherSubject
                    {
                        UserId = userId,
                        TeacherId = teacher.Id,
                        SubjectId = subjectId
                    };
                    _context.TeacherSubjects.Add(link);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(teacher);
        }

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var userId = GetUserId();
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (teacher == null) return NotFound();

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}