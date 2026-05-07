namespace lektion.Models
{
    public class RecipeComponent
    {
        public int component_id { get; set; }
        public int recipe_id { get; set; }
        public int material_id { get; set; }
        public decimal percentage { get; set; }
        public int load_order { get; set; }
    }
}
