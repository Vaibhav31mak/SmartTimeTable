using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }

        // --- ADD THIS METHOD ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure TimetableEntry relationships to STOP Cascade Delete

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.Batch)
                .WithMany()
                .HasForeignKey(t => t.BatchId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent auto-delete

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.Room)
                .WithMany()
                .HasForeignKey(t => t.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.TimeSlot)
                .WithMany()
                .HasForeignKey(t => t.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.TeacherSubject)
                .WithMany()
                .HasForeignKey(t => t.TeacherSubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Department)
                .WithMany()
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}