namespace lektion.Models
{
    public class Deviation
    {
        public int deviation_id { get; set; }
        public int batch_id { get; set; }
        public int? step_execution_id { get; set; }
        public string description { get; set; } = string.Empty;
        public string severity { get; set; } = string.Empty;
        public DateTime created_at { get; set; } = DateTime.Now;
        public int? created_by { get; set; }
    }
}
