using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.AI;

namespace WebApplication1.Services
{
    public class TimeTableGeneratorService
    {
        private readonly AppDbContext _context;

        public TimeTableGeneratorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GenerateTimetableAsync(int batchId)
        {
            // 1. Clean old data
            var oldEntries = _context.TimetableEntries.Where(t => t.BatchId == batchId);
            _context.TimetableEntries.RemoveRange(oldEntries);
            await _context.SaveChangesAsync();

            // 2. Fetch Data
            var batch = await _context.Batches.FindAsync(batchId);
            if (batch == null) return false;

            var subjects = await _context.Subjects
                .Where(s => s.SemesterId == batch.SemesterId && s.DepartmentId == batch.DepartmentId)
                .ToListAsync();
            var slots = await _context.TimeSlots.Where(t => !t.IsLunchBreak).ToListAsync();
            var rooms = await _context.Rooms.ToListAsync();

            var subjectIds = subjects.Select(s => s.Id).ToList();
            var teacherMappings = await _context.TeacherSubjects
                .Where(ts => subjectIds.Contains(ts.SubjectId))
                .ToListAsync();

            // 3. Build Locks
            var existingSchedule = await _context.TimetableEntries
                .Where(t => t.BatchId != batchId)
                .Include(t => t.TeacherSubject)
                .ToListAsync();

            var lockedTeachers = new HashSet<string>(existingSchedule.Select(t => $"{t.DayOfWeek}-{t.TimeSlotId}-{t.TeacherSubject.TeacherId}"));
            var lockedRooms = new HashSet<string>(existingSchedule.Select(t => $"{t.DayOfWeek}-{t.TimeSlotId}-{t.RoomId}"));

            // 4. Run AI
            var ga = new GeneticAlgorithm(
                subjects,
                teacherMappings,
                slots,
                rooms,
                lockedTeachers,
                lockedRooms,
                batchId,
                batch.capacity // <--- Added the missing argument here
            );

            // Run 500 generations
            DNA bestResult = ga.RunEvolution(5000);

            // 5. SAVE WHATEVER WE FOUND
            // Note: We removed the "Fitness > 1.0" check so you can debug conflicts visually
            if (bestResult != null && bestResult.Genes.Any())
            {
                Console.WriteLine($"Best Fitness: {bestResult.Fitness}");

                foreach (var gene in bestResult.Genes)
                {
                    _context.TimetableEntries.Add(new TimetableEntry
                    {
                        BatchId = batchId,
                        DayOfWeek = gene.Day,
                        TimeSlotId = gene.SlotId,
                        TeacherSubjectId = gene.TeacherSubjectId,
                        RoomId = gene.RoomId
                    });
                }
                await _context.SaveChangesAsync();
                return true; // Success!
            }

            return false; // Only fails if NO DNA was created at all
        }
    }
}