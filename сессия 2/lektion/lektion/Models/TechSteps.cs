namespace lektion.Models
{
    public class TechSteps
    {
        public int step_id { get; set; }
        public int techcard_id { get; set; }
        public int step_number { get; set; }
        public string step_name { get; set; } = string.Empty;
        public int? equipment_id { get; set; }
        public string? planned_value { get; set; }
        public string? tolerance_min { get; set; }
        public string? tolerance_max { get; set; }
        public string? instruction { get; set; }
        public int sort_order { get; set; }
    }
}
