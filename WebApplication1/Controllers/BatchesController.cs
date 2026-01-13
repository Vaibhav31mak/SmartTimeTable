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
    public class BatchesController : ControllerBase
    {
        private readonly AppDbContext _context; // Using AppDbContext

        public BatchesController(AppDbContext context)
        {
            _context = context;
        }

        // Helper: Get User ID
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

        // GET: api/Batches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Batch>>> GetBatches()
        {
            var userId = GetUserId();
            return await _context.Batches
                .Where(b => b.UserId == userId)
                .Include(b => b.Department) // Optional: Load related data
                .Include(b => b.Semester)
                .ToListAsync();
        }

        // GET: api/Batches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Batch>> GetBatch(int id)
        {
            var userId = GetUserId();
            var batch = await _context.Batches
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (batch == null) return NotFound();
            return batch;
        }

        // POST: api/Batches
        [HttpPost]
        public async Task<ActionResult<Batch>> CreateBatch(Batch batch)
        {
            batch.UserId = GetUserId(); // Stamp Owner
            _context.Batches.Add(batch);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBatch), new { id = batch.Id }, batch);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBatch(int id, Batch batch)
        {
            if (id != batch.Id) return BadRequest();
            var userId = GetUserId();

            var existing = await _context.Batches.FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);
            if (existing == null) return NotFound();

            existing.Name = batch.Name;
            existing.capacity = batch.capacity;
            existing.SemesterId = batch.SemesterId;
            existing.DepartmentId = batch.DepartmentId;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        // DELETE: api/Batches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatch(int id)
        {
            var userId = GetUserId();
            var batch = await _context.Batches
                .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (batch == null) return NotFound();

            _context.Batches.Remove(batch);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}