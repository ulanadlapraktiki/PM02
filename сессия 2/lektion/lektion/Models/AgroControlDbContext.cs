using lektion.Models;
using Microsoft.EntityFrameworkCore;

namespace lektion.Models
{
    public class AgroControlDbContext : DbContext
    {
        public AgroControlDbContext(DbContextOptions<AgroControlDbContext> options)
            : base(options)
        { }

        // DbSet для всех таблиц
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RawMaterials> RawMaterials { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Receipe> Recipes { get; set; }
        public DbSet<RecipeComponent> RecipeComponents { get; set; }
        public DbSet<TechCards> TechCards { get; set; }
        public DbSet<TechSteps> TechSteps { get; set; }
        public DbSet<ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ProductionBatch> ProductionBatches { get; set; }
        public DbSet<RawBatch> RawBatches { get; set; }
        public DbSet<BatchMaterial> BatchMaterials { get; set; }
        public DbSet<BatchStep> BatchSteps { get; set; }
        public DbSet<LabTest> LabTests { get; set; }
        public DbSet<TestParameters> TestParameters { get; set; }
        public DbSet<Deviation> Deviations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ========== Users ==========
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.user_id);
                entity.HasIndex(e => e.username).IsUnique();
                entity.Property(e => e.user_id).HasColumnName("user_id");
                entity.Property(e => e.username).HasColumnName("username");
                entity.Property(e => e.password_hash).HasColumnName("password_hash");
                entity.Property(e => e.full_name).HasColumnName("full_name");
                entity.Property(e => e.role).HasColumnName("role");
                entity.Property(e => e.department).HasColumnName("department");
                entity.Property(e => e.created_at).HasColumnName("created_at");
            });

            // ========== Products ==========
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(e => e.product_id);
                entity.HasIndex(e => e.product_code).IsUnique();
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.product_code).HasColumnName("product_code");
                entity.Property(e => e.product_name).HasColumnName("product_name");
                entity.Property(e => e.product_type).HasColumnName("product_type");
                entity.Property(e => e.status).HasColumnName("status");
            });

            // ========== RawMaterials ==========
            modelBuilder.Entity<RawMaterials>(entity =>
            {
                entity.ToTable("RawMaterials");
                entity.HasKey(e => e.material_id);
                entity.HasIndex(e => e.material_code).IsUnique();
                entity.Property(e => e.material_id).HasColumnName("material_id");
                entity.Property(e => e.material_code).HasColumnName("material_code");
                entity.Property(e => e.material_name).HasColumnName("material_name");
                entity.Property(e => e.unit).HasColumnName("unit");
            });

            // ========== Equipment ==========
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.ToTable("Equipment");
                entity.HasKey(e => e.equipment_id);
                entity.Property(e => e.equipment_id).HasColumnName("equipment_id");
                entity.Property(e => e.line).HasColumnName("line");
            });

            // ========== Recipes ==========
            modelBuilder.Entity<Receipe>(entity =>
            {
                entity.ToTable("Recipes");
                entity.HasKey(e => e.recipe_id);
                entity.Property(e => e.recipe_id).HasColumnName("recipe_id");
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.version).HasColumnName("version");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.is_active).HasColumnName("is_active");
                entity.Property(e => e.created_at).HasColumnName("created_at");
            });

            // ========== RecipeComponents ==========
            modelBuilder.Entity<RecipeComponent>(entity =>
            {
                entity.ToTable("RecipeComponents");
                entity.Property(e => e.recipe_id).HasColumnName("recipe_id");
                entity.Property(e => e.material_id).HasColumnName("material_id");
                entity.Property(e => e.percentage).HasColumnName("percentage");
                entity.Property(e => e.load_order).HasColumnName("load_order");
            });

            // ========== TechCards ==========
            modelBuilder.Entity<TechCards>(entity =>
            {
                entity.ToTable("TechCards");
                entity.Property(e => e.version).HasColumnName("version");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.is_active).HasColumnName("is_active");
                entity.Property(e => e.created_at).HasColumnName("created_at");

            });

            // ========== TechSteps ==========
            modelBuilder.Entity<TechSteps>(entity =>
            {
                entity.ToTable("TechSteps");
                entity.HasKey(e => e.step_id);
                entity.Property(e => e.step_id).HasColumnName("step_id");
                entity.Property(e => e.step_number).HasColumnName("step_number");
                entity.Property(e => e.step_name).HasColumnName("step_name");
                entity.Property(e => e.equipment_id).HasColumnName("equipment_id");
                entity.Property(e => e.planned_value).HasColumnName("planned_value");
                entity.Property(e => e.tolerance_min).HasColumnName("tolerance_min");
                entity.Property(e => e.tolerance_max).HasColumnName("tolerance_max");
                entity.Property(e => e.instruction).HasColumnName("instruction");
                entity.Property(e => e.sort_order).HasColumnName("sort_order");

            });

            // ========== ProductionOrders ==========
            modelBuilder.Entity<ProductionOrder>(entity =>
            {
                entity.ToTable("ProductionOrders");
                entity.Property(e => e.order_number).HasColumnName("order_number");
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.planned_quantity).HasColumnName("planned_quantity");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.recipe_id).HasColumnName("recipe_id");
                entity.Property(e => e.created_at).HasColumnName("created_at");

            });

            // ========== ProductionBatches ==========
            modelBuilder.Entity<ProductionBatch>(entity =>
            {
                entity.ToTable("ProductionBatches");
                entity.Property(e => e.batch_number).HasColumnName("batch_number");
                entity.Property(e => e.product_id).HasColumnName("product_id");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.start_time).HasColumnName("start_time");
                entity.Property(e => e.end_time).HasColumnName("end_time");
                entity.Property(e => e.lab_decision).HasColumnName("lab_decision");
            });

            // ========== RawBatches ==========
            modelBuilder.Entity<RawBatch>(entity =>
            {
                entity.ToTable("RawBatches");
                entity.Property(e => e.batch_number).HasColumnName("batch_number");
                entity.Property(e => e.material_id).HasColumnName("material_id");
                entity.Property(e => e.supplier).HasColumnName("supplier");
                entity.Property(e => e.quantity).HasColumnName("quantity");
                entity.Property(e => e.receipt_date).HasColumnName("receipt_date");
                entity.Property(e => e.lab_status).HasColumnName("lab_status");
            });

            // ========== BatchMaterials ==========
            modelBuilder.Entity<BatchMaterial>(entity =>
            {
                entity.ToTable("BatchMaterials");
                entity.HasKey(e => e.usage_id);
                entity.Property(e => e.usage_id).HasColumnName("usage_id");
                entity.Property(e => e.batch_id).HasColumnName("batch_id");
                entity.Property(e => e.raw_batch_id).HasColumnName("raw_batch_id");
                entity.Property(e => e.quantity_used).HasColumnName("quantity_used");
            });

            // ========== BatchSteps ==========
            modelBuilder.Entity<BatchStep>(entity =>
            {
                entity.ToTable("BatchSteps");
                entity.HasKey(e => e.execution_id);
                entity.Property(e => e.execution_id).HasColumnName("execution_id");
                entity.Property(e => e.batch_id).HasColumnName("batch_id");
                entity.Property(e => e.step_id).HasColumnName("step_id");
                entity.Property(e => e.actual_value).HasColumnName("actual_value");
                entity.Property(e => e.started_by).HasColumnName("started_by");
                entity.Property(e => e.started_at).HasColumnName("started_at");
                entity.Property(e => e.completed_by).HasColumnName("completed_by");
                entity.Property(e => e.completed_at).HasColumnName("completed_at");
                entity.Property(e => e.is_completed).HasColumnName("is_completed");
            });

            // ========== LabTests ==========
            modelBuilder.Entity<LabTest>(entity =>
            {
                entity.ToTable("LabTests");
                entity.HasIndex(e => e.test_number).IsUnique();
                entity.Property(e => e.test_number).HasColumnName("test_number");
                entity.Property(e => e.target_id).HasColumnName("target_id");
                entity.Property(e => e.status).HasColumnName("status");
                entity.Property(e => e.decision).HasColumnName("decision");
                entity.Property(e => e.decision_comment).HasColumnName("decision_comment");
                entity.Property(e => e.created_at).HasColumnName("created_at");
            });

            // ========== TestParameters ==========
            modelBuilder.Entity<TestParameters>(entity =>
            {
                entity.ToTable("TestParameters");
                entity.Property(e => e.standard_min).HasColumnName("standard_min");
                entity.Property(e => e.standard_max).HasColumnName("standard_max");
            });

            // ========== Deviations ==========
            modelBuilder.Entity<Deviation>(entity =>
            {
                entity.ToTable("Deviations");
                entity.HasKey(e => e.deviation_id);
                entity.Property(e => e.deviation_id).HasColumnName("deviation_id");
                entity.Property(e => e.step_execution_id).HasColumnName("step_execution_id");
                entity.Property(e => e.severity).HasColumnName("severity");
                entity.Property(e => e.description).HasColumnName("description");
                entity.Property(e => e.created_at).HasColumnName("created_at");
                entity.Property(e => e.created_by).HasColumnName("created_by");

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}