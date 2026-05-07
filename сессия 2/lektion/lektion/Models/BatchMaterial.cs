namespace lektion.Models
{
    public class BatchMaterial
    {
        public int usage_id { get; set; }
        public int batch_id { get; set; }
        public int raw_batch_id { get; set; }
        public decimal quantity_used { get; set; }
    }
}
