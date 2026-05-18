namespace lektion.Models
{
    public class ProductionBatch
    {
        public int batch_id { get; set; }
        public string batch_number { get; set; } = string.Empty;
        public int order_id { get; set; }
        public int product_id { get; set; }
        public string status { get; set; } = string.Empty;
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public string? lab_decision { get; set; }
    }
}
