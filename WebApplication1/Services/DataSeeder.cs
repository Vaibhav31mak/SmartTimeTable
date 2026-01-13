using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public static class DataSeeder
    {
        // Must be Async to handle User Creation
        public static async Task SeedAsync(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            // 1. Create the Admin User (The Owner of the Data)
            var user = new IdentityUser
            {
                UserName = "admin@test.com",
                Email = "admin@test.com",
                EmailConfirmed = true
            };

            // Check if user exists
            var existingUser = await userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                await userManager.CreateAsync(user, "Admin@123"); // Password
                existingUser = await userManager.FindByEmailAsync(user.Email);
            }

            string adminId = existingUser.Id; // <--- WE NEED THIS FOR EVERYTHING BELOW

            // 2. Check if Departments exist (If yes, we assume data is already there)
            if (context.Departments.IgnoreQueryFilters().Any(d => d.UserId == adminId)) return;
            // ==========================================
            // GLOBAL DATA (All stamped with adminId)
            // ==========================================

            var dept = new Department { Name = "Computer Science", Code = "CS", UserId = adminId };
            context.Departments.Add(dept);
            await context.SaveChangesAsync();

            var sem = new Semester { Name = "Semester 5", UserId = adminId };
            context.Semesters.Add(sem);
            await context.SaveChangesAsync();

            var rooms = new List<Room>
            {
                new Room { Name = "C-101", capacity = 60, isLab = false, UserId = adminId },
                new Room { Name = "C-102", capacity = 60, isLab = false, UserId = adminId },
                new Room { Name = "L-201 (Lab)", capacity = 30, isLab = true, UserId = adminId }
            };
            context.Rooms.AddRange(rooms);
            await context.SaveChangesAsync();

            var slots = new List<TimeSlot>
            {
                new TimeSlot { StartTime = new TimeSpan(9,0,0), EndTime = new TimeSpan(10,0,0), IsLunchBreak = false, UserId = adminId },
                new TimeSlot { StartTime = new TimeSpan(10,0,0), EndTime = new TimeSpan(11,0,0), IsLunchBreak = false, UserId = adminId },
                new TimeSlot { StartTime = new TimeSpan(11,0,0), EndTime = new TimeSpan(11,30,0), IsLunchBreak = true, UserId = adminId }, // Break
                new TimeSlot { StartTime = new TimeSpan(11,30,0), EndTime = new TimeSpan(12,30,0), IsLunchBreak = false, UserId = adminId },
                new TimeSlot { StartTime = new TimeSpan(12,30,0), EndTime = new TimeSpan(13,30,0), IsLunchBreak = false, UserId = adminId }
            };
            context.TimeSlots.AddRange(slots);
            await context.SaveChangesAsync();

            var t1 = new Teacher { Name = "Mr. John (C#)", DepartmentId = dept.Id, UserId = adminId };
            var t2 = new Teacher { Name = "Ms. Sarah (Web)", DepartmentId = dept.Id, UserId = adminId };
            var t3 = new Teacher { Name = "Dr. Mike (DB)", DepartmentId = dept.Id, UserId = adminId };
            var t4 = new Teacher { Name = "Mrs. Emma (Cloud/Sec)", DepartmentId = dept.Id, UserId = adminId };
            context.Teachers.AddRange(t1, t2, t3, t4);
            await context.SaveChangesAsync();

            // ==========================================
            // BATCH A (CS-A)
            // ==========================================

            var batchA = new Batch { Name = "CS-A", DepartmentId = dept.Id, SemesterId = sem.Id, capacity = 60, UserId = adminId };
            context.Batches.Add(batchA);
            await context.SaveChangesAsync();

            var subjectsA = new List<Subject>
            {
                new Subject { Name = "C# Programming", Code = "CS501", WeeklyLectures = 3, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "Web Dev (Angular)", Code = "CS502", WeeklyLectures = 3, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "Database (SQL)", Code = "CS503", WeeklyLectures = 3, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "Cloud Computing", Code = "CS504", WeeklyLectures = 2, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "Cyber Security", Code = "CS505", WeeklyLectures = 2, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "C# Lab", Code = "CS501-L", WeeklyLectures = 2, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = true, UserId = adminId }
            };
            context.Subjects.AddRange(subjectsA);
            await context.SaveChangesAsync();

            var mappingsA = new List<TeacherSubject>
            {
                new TeacherSubject { TeacherId = t1.Id, SubjectId = subjectsA[0].Id, UserId = adminId }, // John -> C#
                new TeacherSubject { TeacherId = t2.Id, SubjectId = subjectsA[1].Id, UserId = adminId }, // Sarah -> Web
                new TeacherSubject { TeacherId = t3.Id, SubjectId = subjectsA[2].Id, UserId = adminId }, // Mike -> DB
                new TeacherSubject { TeacherId = t4.Id, SubjectId = subjectsA[3].Id, UserId = adminId }, // Emma -> Cloud
                new TeacherSubject { TeacherId = t4.Id, SubjectId = subjectsA[4].Id, UserId = adminId }, // Emma -> Security
                new TeacherSubject { TeacherId = t1.Id, SubjectId = subjectsA[5].Id, UserId = adminId }  // John -> Lab
            };
            context.TeacherSubjects.AddRange(mappingsA);
            await context.SaveChangesAsync();

            // ==========================================
            // BATCH B (CS-B)
            // ==========================================

            var batchB = new Batch { Name = "CS-B", DepartmentId = dept.Id, SemesterId = sem.Id, capacity = 60, UserId = adminId };
            context.Batches.Add(batchB);
            await context.SaveChangesAsync();

            var subjectsB = new List<Subject>
            {
                new Subject { Name = "Java Programming", Code = "CS506", WeeklyLectures = 4, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId },
                new Subject { Name = "Operating Systems", Code = "CS507", WeeklyLectures = 4, DepartmentId = dept.Id, SemesterId = sem.Id, IsLab = false, UserId = adminId }
            };
            context.Subjects.AddRange(subjectsB);
            await context.SaveChangesAsync();

            var mappingsB = new List<TeacherSubject>
            {
                new TeacherSubject { TeacherId = t1.Id, SubjectId = subjectsB[0].Id, UserId = adminId },
                new TeacherSubject { TeacherId = t3.Id, SubjectId = subjectsB[1].Id, UserId = adminId }
            };
            context.TeacherSubjects.AddRange(mappingsB);
            await context.SaveChangesAsync();
        }
    }
}
/*
{
  "email": "admin@test.com",
  "password": "Admin@123"
}
 */