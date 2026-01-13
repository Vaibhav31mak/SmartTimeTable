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
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            var userId = GetUserId();
            return await _context.Departments.Where(d => d.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment(Department department)
        {
            department.UserId = GetUserId();
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDepartments), new { id = department.Id }, department);
        }

        // ... existing code ...

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, Department department)
        {
            if (id != department.Id) return BadRequest();

            var userId = GetUserId();

            // Security Check: Ensure the item belongs to the user before editing
            var existing = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);

            if (existing == null) return NotFound();

            // Update fields
            existing.Name = department.Name;
            existing.Code = department.Code;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var userId = GetUserId();
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id && d.UserId == userId);
            if (dept == null) return NotFound();

            _context.Departments.Remove(dept);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}