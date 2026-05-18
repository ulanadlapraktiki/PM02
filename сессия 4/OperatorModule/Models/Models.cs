using System;

namespace OperatorModule.Models
{

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
    }
    public class ProductionBatch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int? OrderId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal ActualQuantityKg { get; set; }
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

    public class CompleteStepRequest
    {
        public int StepId { get; set; }
        public decimal? ActualTempC { get; set; }
        public int? ActualDurationMin { get; set; }
        public decimal? ActualPressureBar { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class ActiveBatchDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int LineNumber { get; set; }
        public string CurrentStep { get; set; } = string.Empty;
        public string BatchStatus { get; set; } = string.Empty;
        public string StepStatus { get; set; } = string.Empty;
        public bool HasWarning { get; set; }
        public bool HasCriticalDeviation { get; set; }
    }
}