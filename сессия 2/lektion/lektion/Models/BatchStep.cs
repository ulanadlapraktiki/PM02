namespace lektion.Models
{
    public class BatchStep
    {
        public int execution_id { get; set; }
        public int batch_id { get; set; }
        public int step_id { get; set; }
        public string? actual_value { get; set; }
        public int? started_by { get; set; }
        public DateTime? started_at { get; set; }
        public int? completed_by { get; set; }
        public DateTime? completed_at { get; set; }
        public bool is_completed { get; set; }
    }
}
