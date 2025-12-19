using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

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
            // 1. CLEAR EXISTING TIMETABLE for this batch (Start fresh)
            var existingEntries = _context.TimetableEntries.Where(t => t.BatchId == batchId);
            _context.TimetableEntries.RemoveRange(existingEntries);
            await _context.SaveChangesAsync();

            // 2. FETCH DATA NEEDED
            var batch = await _context.Batches.FindAsync(batchId);
            var subjects = await _context.Subjects
                                         .Where(s => s.SemesterId == batch.SemesterId && s.DepartmentId == batch.DepartmentId)
                                         .ToListAsync();

            var timeSlots = await _context.TimeSlots
                                          .Where(t => !t.IsLunchBreak) // Ignore lunch breaks
                                          .ToListAsync();

            var rooms = await _context.Rooms.ToListAsync();

            // 3. START SCHEDULING
            var random = new Random();
            int daysInWeek = 5; // Mon-Fri

            foreach (var subject in subjects)
            {
                int lecturesScheduled = 0;

                // Loop until we meet the weekly requirement (e.g., 4 lectures of C#)
                while (lecturesScheduled < subject.WeeklyLectures)
                {
                    bool scheduled = false;
                    int attempts = 0;

                    // Try to find a slot (Max 20 attempts to prevent infinite loops)
                    while (!scheduled && attempts < 20)
                    {
                        attempts++;

                        // A. Pick Random Day & Slot
                        int day = random.Next(1, daysInWeek + 1);
                        var slot = timeSlots[random.Next(timeSlots.Count)];

                        // B. Find a Teacher for this Subject
                        var teacherSubject = await _context.TeacherSubjects
                                            .Include(ts => ts.Teacher)
                                            .FirstOrDefaultAsync(ts => ts.SubjectId == subject.Id);

                        if (teacherSubject == null) break; // No teacher assigned? Skip.

                        // C. Pick a Room (Just pick first available or random)
                        var room = rooms[random.Next(rooms.Count)];

                        // D. CHECK CONSTRAINTS (The "Smart" Part)

                        // 1. Is this Batch already busy at this time?
                        bool batchBusy = await _context.TimetableEntries.AnyAsync(t =>
                            t.DayOfWeek == day && t.TimeSlotId == slot.Id && t.BatchId == batchId);

                        // 2. Is the Teacher busy elsewhere?
                        bool teacherBusy = await _context.TimetableEntries.AnyAsync(t =>
                            t.DayOfWeek == day && t.TimeSlotId == slot.Id && t.TeacherSubject.TeacherId == teacherSubject.TeacherId);

                        // 3. Is the Room occupied?
                        bool roomBusy = await _context.TimetableEntries.AnyAsync(t =>
                            t.DayOfWeek == day && t.TimeSlotId == slot.Id && t.RoomId == room.Id);

                        // E. IF SAFE -> BOOK IT
                        if (!batchBusy && !teacherBusy && !roomBusy)
                        {
                            var entry = new TimetableEntry
                            {
                                BatchId = batchId,
                                DayOfWeek = day,
                                TimeSlotId = slot.Id,
                                TeacherSubjectId = teacherSubject.Id,
                                RoomId = room.Id
                            };

                            _context.TimetableEntries.Add(entry);
                            await _context.SaveChangesAsync(); // Save immediately to block this slot for next checks

                            lecturesScheduled++;
                            scheduled = true;
                        }
                    }
                }
            }

            return true;
        }
    }
}