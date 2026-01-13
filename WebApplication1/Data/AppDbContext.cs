using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IUserResolverService _userResolver;

        public AppDbContext(DbContextOptions<AppDbContext> options, IUserResolverService userResolver)
            : base(options)
        {
            _userResolver = userResolver;
        }

        // --- FIX PART 1: Create a Property that gets the LIVE user ---
        // EF Core can read this property every time it runs a query.
        public string CurrentUserId => _userResolver.GetUser();

        public DbSet<Department> Departments { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<TimetableEntry> TimetableEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- FIX PART 2: Use the Property, NOT a variable ---
            // Notice we use 'this.CurrentUserId'. 
            // This tells EF: "Check the CurrentUserId property every time you run this SQL".

            modelBuilder.Entity<Department>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<Semester>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<Room>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<TimeSlot>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<Subject>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<Teacher>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<Batch>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<TeacherSubject>().HasQueryFilter(x => x.UserId == this.CurrentUserId);
            modelBuilder.Entity<TimetableEntry>().HasQueryFilter(x => x.UserId == this.CurrentUserId);

            // Relationship Configurations (Keep these as they were)
            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.Batch).WithMany().HasForeignKey(t => t.BatchId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.Room).WithMany().HasForeignKey(t => t.RoomId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.TimeSlot).WithMany().HasForeignKey(t => t.TimeSlotId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TimetableEntry>()
                .HasOne(t => t.TeacherSubject).WithMany().HasForeignKey(t => t.TeacherSubjectId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Department).WithMany().HasForeignKey(s => s.DepartmentId).OnDelete(DeleteBehavior.Restrict);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && e.State == EntityState.Added);

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;
                if (string.IsNullOrEmpty(entity.UserId))
                {
                    entity.UserId = _userResolver.GetUser();
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}