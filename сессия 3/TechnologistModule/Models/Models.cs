using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace TechnologistModule.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Department { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
        public BitmapImage? AvatarImage { get; set; }
    }

    public class LoginResponse
    {
        public string message { get; set; } = string.Empty;
        public int userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public string department { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Status { get; set; } = "draft";
        public DateTime CreatedAt { get; set; }
        public int? ProductId { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class TechMap
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Status { get; set; } = "draft";
        public int? ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }
    }

    public class TechMapStep
    {
        public int Id { get; set; }
        public int TechMapId { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Instruction { get; set; } = string.Empty;
        public int? DurationMin { get; set; }
        public bool IsMandatory { get; set; } = true;
        public int? EquipmentId { get; set; }
    }

    public class ProductionOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int? RecipeId { get; set; }
        public decimal PlannedQuantityKg { get; set; }
        public string Status { get; set; } = "planned";
        public DateTime? PlannedStartDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class ProductionBatch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = "planned";
        public decimal ActualQuantityKg { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class ProductionStep
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int StepOrder { get; set; }
        public string StepName { get; set; } = string.Empty;
        public decimal? PlannedTempC { get; set; }
        public decimal? ActualTempC { get; set; }
        public int? PlannedDurationMin { get; set; }
        public int? ActualDurationMin { get; set; }
        public decimal? PlannedPressureBar { get; set; }
        public decimal? ActualPressureBar { get; set; }
        public bool DeviationFlag { get; set; }
        public string OperatorComment { get; set; } = string.Empty;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class ExtruderProgram
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? RecipeId { get; set; }
        public int? EquipmentId { get; set; }
        public string Status { get; set; } = "draft";
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class ExtruderProgramZone
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int ZoneNumber { get; set; }
        public decimal TargetTemperature { get; set; }
        public decimal Tolerance { get; set; }
        public decimal TargetPressure { get; set; }
        public int TargetSpeed { get; set; }
    }

    public class Deviation
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public int? StepId { get; set; }
        public string DeviationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = "medium";
        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolutionComment { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
    }

    public class QualityControl
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public DateTime AnalysisDate { get; set; }
        public string SampleType { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public decimal? MeasuredValue { get; set; }
        public string StandardValue { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string AnalystComment { get; set; } = string.Empty;
        public int? AnalystId { get; set; }
        public int? CreatedBy { get; set; }
    }

    public class DashboardData
    {
        public int ActiveProducts { get; set; }
        public int ActiveRecipes { get; set; }
        public int BatchesInProgress { get; set; }
        public int PendingQualityControl { get; set; }
        public int DeviationsToday { get; set; }
        public int UnresolvedDeviations { get; set; }
    }

    public class BatchReportItem
    {
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ActualQuantityKg { get; set; }
    }

    public class Equipment
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "operational";
    }

    public class MaterialDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = "kg";
        public decimal? DefaultMinValue { get; set; }
        public decimal? DefaultMaxValue { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RecipeComponent
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int MaterialId { get; set; }
        public decimal Percentage { get; set; }
        public decimal Tolerance { get; set; }
        public int OrderIndex { get; set; }
    }
}