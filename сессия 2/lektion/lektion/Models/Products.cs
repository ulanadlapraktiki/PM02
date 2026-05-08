namespace lektion.Models
{
    public class Product
    {
        public int product_id { get; set; }
        public string product_code { get; set; } = string.Empty;
        public string product_name { get; set; } = string.Empty;
        public string product_type { get; set; } = string.Empty;
        public string status { get; set; } = "active";
    }
}
