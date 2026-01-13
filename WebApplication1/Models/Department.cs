namespace WebApplication1.Models
{
    public class Department : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., Computer Science
        public string Code { get; set; } // e.g., CS
    }
}
