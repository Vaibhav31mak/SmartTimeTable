namespace WebApplication1.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } // e.g., Room 101
        public int capacity { get; set; } // e.g., 50
        public bool isLab { get; set; } // e.g., true if it's a lab room
    }
}
