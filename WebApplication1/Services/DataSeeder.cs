using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // 1. Check if data exists. If yes, do nothing.
            if (context.Departments.Any()) return;

            // 2. Add Department
            var dept = new Department { Name = "Computer Science", Code = "CS" };
            context.Departments.Add(dept);
            context.SaveChanges(); // Save to get the ID

            // 3. Add Semester
            var sem = new Semester { Name = "Semester 5" };
            context.Semesters.Add(sem);
            context.SaveChanges();

            // 4. Add Rooms
            var rooms = new List<Room>
            {
                new Room { Name = "C-101", capacity = 60, isLab = false },
                new Room { Name = "C-102", capacity = 60, isLab = false },
                new Room { Name = "L-201 (Lab)", capacity = 30, isLab = true }
            };
            context.Rooms.AddRange(rooms);
            context.SaveChanges();

            // 5. Add TimeSlots (9 to 1 PM)
            var slots = new List<TimeSlot>
            {
                new TimeSlot { StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(10,0,0), IsLunchBreak = false },
                new TimeSlot { StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(11,0,0), IsLunchBreak = false },
                new TimeSlot { StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(11,30,0), IsLunchBreak = true }, // Break
                new TimeSlot { StartTime = new TimeSpan(11,30,0), EndTime = new TimeSpan(12,30,0), IsLunchBreak = false },
                new TimeSlot { StartTime = new TimeSpan(12,30,0), EndTime = new TimeSpan(13,30,0), IsLunchBreak = false }
            };
            context.TimeSlots.AddRange(slots);
            context.SaveChanges();

            // 6. Add Batch
            var batch = new Batch { Name = "CS-A", DepartmentId = dept.Id, SemesterId = sem.Id };
            context.Batches.Add(batch);
            context.SaveChanges();

            // 7. Add Subjects (Linked to Dept & Sem)
            var subjects = new List<Subject>
            {
                new Subject { Name = "C# Programming", Code = "CS501", WeeklyLectures = 4, DepartmentId = dept.Id, SemesterId = sem.Id },
                new Subject { Name = "Web Dev (Angular)", Code = "CS502", WeeklyLectures = 3, DepartmentId = dept.Id, SemesterId = sem.Id },
                new Subject { Name = "Database (SQL)", Code = "CS503", WeeklyLectures = 3, DepartmentId = dept.Id, SemesterId = sem.Id },
                new Subject { Name = "Cloud Computing", Code = "CS504", WeeklyLectures = 2, DepartmentId = dept.Id, SemesterId = sem.Id },
                new Subject { Name = "Cyber Security", Code = "CS505", WeeklyLectures = 2, DepartmentId = dept.Id, SemesterId = sem.Id }
            };
            context.Subjects.AddRange(subjects);
            context.SaveChanges();

            // 8. Add Teachers
            var t1 = new Teacher { Name = "Mr. John (C#)", DepartmentId = dept.Id };
            var t2 = new Teacher { Name = "Ms. Sarah (Web)", DepartmentId = dept.Id };
            var t3 = new Teacher { Name = "Dr. Mike (DB)", DepartmentId = dept.Id };
            var t4 = new Teacher { Name = "Mrs. Emma (Cloud/Sec)", DepartmentId = dept.Id };

            context.Teachers.AddRange(t1, t2, t3, t4);
            context.SaveChanges();

            // 9. Assign Teachers to Subjects (Who teaches what?)
            var mappings = new List<TeacherSubject>
            {
                new TeacherSubject { TeacherId = t1.Id, SubjectId = subjects[0].Id }, // John teaches C#
                new TeacherSubject { TeacherId = t2.Id, SubjectId = subjects[1].Id }, // Sarah teaches Web
                new TeacherSubject { TeacherId = t3.Id, SubjectId = subjects[2].Id }, // Mike teaches DB
                new TeacherSubject { TeacherId = t4.Id, SubjectId = subjects[3].Id }, // Emma teaches Cloud
                new TeacherSubject { TeacherId = t4.Id, SubjectId = subjects[4].Id }  // Emma teaches Security
            };
            context.TeacherSubjects.AddRange(mappings);
            context.SaveChanges();
        }
    }
}