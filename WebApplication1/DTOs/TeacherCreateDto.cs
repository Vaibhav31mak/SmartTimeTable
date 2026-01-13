namespace WebApplication1.Models.DTOs
{
    public class TeacherCreateDto
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }

        // The list of Subjects this teacher can teach
        public List<int> SubjectIds { get; set; } = new List<int>();
    }
}