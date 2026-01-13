using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Required for finding User ID

namespace WebApplication1.Controllers
{
    [Authorize]
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
            // 1. Try to generate the timetable
            var result = await _generatorService.GenerateTimetableAsync(batchId);

            if (result)
            {
                return Ok(new { message = "Timetable Generated Successfully!" });
            }
            else
            {
                // 2. DIAGNOSTICS: If it fails, let's find out why.

                // A. Who is the user?
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // B. Does the requested Batch exist for this user?
                var batchExists = await _context.Batches.AnyAsync(b => b.Id == batchId);

                // C. Count the data available to THIS user
                // (The Global Filter will apply automatically, so we see what the logic sees)
                var roomCount = await _context.Rooms.CountAsync();
                var teacherCount = await _context.Teachers.CountAsync();
                var subjectCount = await _context.Subjects.CountAsync();
                var slotCount = await _context.TimeSlots.CountAsync();

                return BadRequest(new
                {
                    Error = "Timetable Generation Failed",
                    Reason = "The algorithm could not find a valid schedule, or data is missing.",
                    DebugInfo = new
                    {
                        YourUserId = currentUserId ?? "NULL (Auth Failed!)",
                        RequestedBatchId = batchId,
                        BatchFound = batchExists,
                        DataVisibleToYou = new
                        {
                            Rooms = roomCount,
                            Teachers = teacherCount,
                            Subjects = subjectCount,
                            TimeSlots = slotCount
                        },
                        Hint = roomCount == 0 ? "You have 0 Rooms. Are you logged in as the Admin who owns the data?" : "Data exists, but constraints might be too tight."
                    }
                });
            }
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
                    .ThenInclude(ts => ts.Subject)
                .Include(t => t.TeacherSubject)
                    .ThenInclude(ts => ts.Teacher)
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