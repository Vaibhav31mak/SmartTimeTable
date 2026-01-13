namespace WebApplication1.Models.DTOs
{
    public class TimeSlotCreateDto
    {
        public string StartTime { get; set; } // "09:00:00"
        public string EndTime { get; set; }   // "10:00:00"
        public bool IsLunchBreak { get; set; }
    }
}