namespace WebApplication1.Models
{
    public class Batch : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., CS-A

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int capacity { get; set; } = 60; // e.g., 60
    }
}