namespace WebApplication1.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., CS-A

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}