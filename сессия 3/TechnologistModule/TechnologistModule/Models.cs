using System;

namespace TechnologistModule
{
    public class LoginResponse
    {
        public int userId { get; set; }
        public string username { get; set; } = string.Empty;
        public string fullName { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
    }

    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string Form { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ProductionOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public decimal PlannedQuantityKg { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? PlannedStartDate { get; set; }
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

    public class Deviation
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string DeviationType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime ReportedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
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
}