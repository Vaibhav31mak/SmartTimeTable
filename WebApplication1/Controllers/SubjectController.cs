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
    public class SubjectsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubjectsController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: api/Subjects
        // GET
        [HttpGet]
        public async Task<ActionResult> GetSubjects()
        {
            var userId = GetUserId();

            var subjects = await _context.Subjects
                .Where(s => s.UserId == userId)
                .Include(s => s.Department).Include(s => s.Semester)
                .ToListAsync();

            var allLinks = await _context.TeacherSubjects
                .Where(ts => ts.UserId == userId)
                .ToListAsync();

            // Manual Join
            var result = subjects.Select(s => new {
                s.Id,
                s.Name,
                s.Code,
                s.WeeklyLectures,
                s.IsLab,
                s.SemesterId,
                s.DepartmentId,
                s.Department,
                s.Semester,
                s.UserId,
                TeacherSubjects = allLinks.Where(l => l.SubjectId == s.Id).ToList()
            });

            return Ok(result);
        }

        // PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectCreateDto dto)
        {
            var userId = GetUserId();
            var existing = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (existing == null) return NotFound();

            // Update Basic
            existing.Name = dto.Name;
            existing.Code = dto.Code;
            existing.WeeklyLectures = dto.WeeklyLectures;
            existing.IsLab = dto.IsLab;
            existing.SemesterId = dto.SemesterId;
            existing.DepartmentId = dto.DepartmentId;

            // Manual Delete Old Links
            var oldLinks = await _context.TeacherSubjects
                .Where(ts => ts.SubjectId == id && ts.UserId == userId)
                .ToListAsync();
            _context.TeacherSubjects.RemoveRange(oldLinks);

            // Add New Links
            if (dto.TeacherIds != null)
            {
                foreach (var tId in dto.TeacherIds)
                {
                    _context.TeacherSubjects.Add(new TeacherSubject
                    {
                        UserId = userId,
                        SubjectId = existing.Id,
                        TeacherId = tId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // POST: api/Subjects (Custom Logic for Checkboxes)
        [HttpPost]
        public async Task<ActionResult<Subject>> CreateSubject([FromBody] SubjectCreateDto dto)
        {
            var userId = GetUserId();

            // 1. Create the Subject
            var subject = new Subject
            {
                UserId = userId,
                Name = dto.Name,
                Code = dto.Code,
                WeeklyLectures = dto.WeeklyLectures,
                IsLab = dto.IsLab,
                SemesterId = dto.SemesterId,
                DepartmentId = dto.DepartmentId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync(); // Generates subject.Id

            // 2. Loop through checked Teacher IDs and create links
            if (dto.TeacherIds != null && dto.TeacherIds.Any())
            {
                foreach (var teacherId in dto.TeacherIds)
                {
                    var link = new TeacherSubject
                    {
                        UserId = userId,
                        TeacherId = teacherId,
                        SubjectId = subject.Id
                    };
                    _context.TeacherSubjects.Add(link);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(subject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var userId = GetUserId();
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);
            if (subject == null) return NotFound();

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}