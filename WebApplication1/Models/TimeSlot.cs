namespace WebApplication1.Models
{
    public class TimeSlot
    {
        public int Id { get; set; }
        public TimeSpan StartTime { get; set; } // e.g., "Monday"
        public TimeSpan EndTime { get; set; } // e.g., 09:00 AM
        public bool IsLunchBreak { get; set; } // e.g., 10:30 AM
    }
}
