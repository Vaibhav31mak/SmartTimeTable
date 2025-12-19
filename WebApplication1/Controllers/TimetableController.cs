using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly TimeTableGeneratorService _generatorService;
        private readonly AppDbContext _context;

        public TimetableController(TimeTableGeneratorService generatorService, AppDbContext context)
        {
            _generatorService = generatorService;
            _context = context;
        }

        // POST: api/timetable/generate/1
        [HttpPost("generate/{batchId}")]
        public async Task<IActionResult> Generate(int batchId)
        {
            var result = await _generatorService.GenerateTimetableAsync(batchId);

            if (result)
                return Ok(new { message = "Timetable Generated Successfully!" });
            else
                return BadRequest("Failed to generate timetable.");
        }

        // GET: api/timetable/1
        [HttpGet("{batchId}")]
        public async Task<IActionResult> GetTimetable(int batchId)
        {
            var timetable = await _context.TimetableEntries
                .Where(t => t.BatchId == batchId)
                .Include(t => t.TimeSlot)
                .Include(t => t.Room)
                .Include(t => t.TeacherSubject)
                    .ThenInclude(ts => ts.Subject) // Get Subject Name
                .Include(t => t.TeacherSubject)
                    .ThenInclude(ts => ts.Teacher) // Get Teacher Name
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.TimeSlot.StartTime)
                .Select(t => new
                {
                    Day = t.DayOfWeek,
                    Slot = t.TimeSlot.StartTime + " - " + t.TimeSlot.EndTime,
                    Subject = t.TeacherSubject.Subject.Name,
                    Teacher = t.TeacherSubject.Teacher.Name,
                    Room = t.Room.Name
                })
                .ToListAsync();

            return Ok(timetable);
        }
    }
}