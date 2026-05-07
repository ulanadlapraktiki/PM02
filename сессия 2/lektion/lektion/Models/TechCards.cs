namespace lektion.Models
{
    public class TechCards
    {
        public int techcard_id { get; set; }
        public int product_id { get; set; }
        public int version { get; set; }
        public string status { get; set; } = string.Empty;
        public bool is_active { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
