using System;
using System.Windows.Media.Imaging;

namespace LaboratoryModule.Models
{
    // ==================== ПОЛЬЗОВАТЕЛИ ====================
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
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

    public class UserSession
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    // ==================== ПАРТИИ ====================
    public class ProductionBatch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ActualQuantityKg { get; set; }
        public int? CreatedBy { get; set; }
    }

    // ==================== СЫРЬЕ ====================
    public class RawMaterialDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string SupplierBatchNumber { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string StorageLocation { get; set; } = string.Empty;
        public string LabStatus { get; set; } = string.Empty;
        public DateTime? LastTestDate { get; set; }
        public int? TestId { get; set; }
    }

    // ==================== ИСПЫТАНИЯ ====================
    public class LaboratoryTest
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchType { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public string TestType { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public int? AssignedTo { get; set; }
        public string AnalystName { get; set; } = string.Empty;
        public string Priority { get; set; } = "normal";
        public string Status { get; set; } = "created";
        public string? Comment { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Decision { get; set; }
        public string? DecisionComment { get; set; }
        public int? DecisionBy { get; set; }
        public DateTime? DecisionAt { get; set; }
    }

    public class TestResult
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public decimal? MeasuredValue { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string? AnalystComment { get; set; }
        public DateTime EnteredAt { get; set; }
        public int? EnteredBy { get; set; }
    }

    // ==================== НОРМАТИВЫ ====================
    public class StandardParameter
    {
        public int Id { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    // ==================== ИСТОРИЯ ====================
    public class TestHistoryItem
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty; 
        public string SampleType { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string AnalystName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    // ==================== DASHBOARD ====================
    public class DashboardData
    {
        public int PendingRawMaterials { get; set; }
        public int PendingFinishedProducts { get; set; }
        public int TestsInProgress { get; set; }
        public int CompletedToday { get; set; }
        public int BlockedBatches { get; set; }
        public int ApprovedBatches { get; set; }
    }

    // ==================== REQUEST MODELS ====================
    public class CreateTestRequest
    {
        public int BatchId { get; set; }
        public string BatchType { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }

    public class AddTestResultRequest
    {
        public string ParameterName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class CompleteTestRequest
    {
        public string Decision { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }

    // ==================== ОТКЛОНЕНИЯ ====================
    public class Deviation
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string DeviationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
    }

    // ==================== ВСПОМОГАТЕЛЬНЫЕ ====================
    public class QualityTest
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
        public string SampleType { get; set; } = string.Empty;
        public string Status { get; set; } = "in_progress";
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string AnalystName { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string? Decision { get; set; }
        public string? Comment { get; set; }
    }

    public class MaterialBatch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string SupplierBatchNumber { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "kg";
        public string StorageLocation { get; set; } = string.Empty;
        public string LabStatus { get; set; } = "awaiting";
        public int? TestId { get; set; }
        public DateTime? LastTestDate { get; set; }
    }
}