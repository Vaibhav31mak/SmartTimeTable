namespace WebApplication1.Models
{
    public abstract class BaseEntity
    {
        // Every table gets this "Owner Stamp"
        public string UserId { get; set; } = string.Empty;
    }
}