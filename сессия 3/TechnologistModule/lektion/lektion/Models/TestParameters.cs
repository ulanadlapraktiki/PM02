namespace lektion.Models
{
    public class TestParameters
    {
        public int param_id { get; set; }
        public int test_id { get; set; }
        public string param_name { get; set; } = string.Empty;
        public string? standard_min { get; set; }
        public string? standard_max { get; set; }
        public string? actual_value { get; set; }
        public bool? is_ok { get; set; }
    }
}
