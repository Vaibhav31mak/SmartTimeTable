namespace WebApplication1.Models
{
    public class TimetableEntry : BaseEntity
    {
        public int Id { get; set; }

        public int DayOfWeek { get; set; } // 1=Mon, 2=Tue...

        public int TimeSlotId { get; set; }
        public TimeSlot TimeSlot { get; set; }

        public int BatchId { get; set; }
        public Batch Batch { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int TeacherSubjectId { get; set; }
        public TeacherSubject TeacherSubject { get; set; }
    }
}
