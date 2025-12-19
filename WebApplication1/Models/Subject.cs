namespace WebApplication1.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., Data Structures
        public string Code { get; set; } // e.g., CS302
        public int WeeklyLectures { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
