namespace CarPark.Models
{
    public class BatchJobRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public BatchJobStatus Status { get; set; }
        public int TotalRows { get; set; }
        public int ProcessedRows { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }

    public enum BatchJobStatus
    {
        Running,
        Completed,
        Failed
    }
}