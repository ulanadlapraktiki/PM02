namespace lektion.Models
{
    public class RawBatch
    {
        public int raw_batch_id { get; set; }
        public string batch_number { get; set; } = string.Empty;
        public int material_id { get; set; }
        public string? supplier { get; set; }
        public decimal quantity { get; set; }
        public DateTime receipt_date { get; set; }
        public string lab_status { get; set; } = string.Empty;
        public string? lab_comment { get; set; }
    }
}
