namespace lektion.Models
{
    public class ProductionOrder
    {
        public int order_id { get; set; }
        public string order_number { get; set; } = string.Empty;
        public int product_id { get; set; }
        public decimal planned_quantity { get; set; }
        public string status { get; set; } = string.Empty;
        public int recipe_id { get; set; }
        public int techcard_id { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
