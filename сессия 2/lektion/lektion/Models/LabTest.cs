namespace lektion.Models
{
    public class LabTest
    {
        public int test_id { get; set; }
        public string test_number { get; set; } = string.Empty;
        public string target_type { get; set; } = string.Empty;
        public int target_id { get; set; }
        public string status { get; set; } = string.Empty;
        public string? decision { get; set; }
        public string? decision_comment { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
