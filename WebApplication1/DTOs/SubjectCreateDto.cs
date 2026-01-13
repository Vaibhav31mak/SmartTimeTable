namespace WebApplication1.Models.DTOs
{
    public class SubjectCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public int WeeklyLectures { get; set; }
        public bool IsLab { get; set; }

        public int SemesterId { get; set; }
        public int DepartmentId { get; set; }

        // The list of Teacher IDs checked in the frontend
        public List<int> TeacherIds { get; set; } = new List<int>();
    }
}