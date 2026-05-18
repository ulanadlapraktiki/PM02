using Microsoft.VisualStudio.TestTools.UnitTesting;
using LaboratoryModule.Models;
using System;
using TestResult = LaboratoryModule.Models.TestResult;

namespace LaboratoryModule.Tests
{
    [TestClass]
    public class LaboratoryModuleTests
    {
        // Проверка валидации значения в норме
        [TestMethod]
        public void Test1()
        {
            var result = new TestResult
            {
                MinValue = 6.5m,
                MaxValue = 7.5m,
                MeasuredValue = 7.0m
            };
            bool isValid = result.MeasuredValue >= result.MinValue && result.MeasuredValue <= result.MaxValue;
            Assert.IsTrue(isValid);
        }

        // Проверка валидации значения ниже нормы
        [TestMethod]
        public void Test2()
        {
            var result = new TestResult
            {
                MinValue = 6.5m,
                MaxValue = 7.5m,
                MeasuredValue = 5.0m
            };
            bool isValid = result.MeasuredValue >= result.MinValue && result.MeasuredValue <= result.MaxValue;
            Assert.IsFalse(isValid);
        }

        // Проверка валидации значения выше нормы
        [TestMethod]
        public void Test3()
        {
            var result = new TestResult
            {
                MinValue = 6.5m,
                MaxValue = 7.5m,
                MeasuredValue = 9.0m
            };
            bool isValid = result.MeasuredValue >= result.MinValue && result.MeasuredValue <= result.MaxValue;
            Assert.IsFalse(isValid);
        }

        // Проверка создания партии сырья
        [TestMethod]
        public void Test4()
        {
            var batch = new RawMaterialDto
            {
                BatchNumber = "RAW-2026-001",
                MaterialName = "Азотная кислота",
                SupplierName = "ХимТрейд",
                Quantity = 5000,
                LabStatus = "awaiting"
            };
            Assert.AreEqual("RAW-2026-001", batch.BatchNumber);
            Assert.AreEqual("Азотная кислота", batch.MaterialName);
            Assert.AreEqual("awaiting", batch.LabStatus);
        }

        // Проверка создания партии готовой продукции
        [TestMethod]
        public void Test5()
        {
            var batch = new ProductionBatch
            {
                BatchNumber = "FG-2026-001",
                ProductName = "Гербицид А",
                Status = "quality_pending",
                ActualQuantityKg = 1500
            };
            Assert.AreEqual("FG-2026-001", batch.BatchNumber);
            Assert.AreEqual("Гербицид А", batch.ProductName);
            Assert.AreEqual("quality_pending", batch.Status);
            Assert.AreEqual(1500, batch.ActualQuantityKg);
        }

        // Проверка создания лабораторного испытания
        [TestMethod]
        public void Test6()
        {
            var test = new LaboratoryTest
            {
                BatchId = 100,
                BatchType = "raw_material",
                Status = "in_progress",
                AnalystName = "Петрова Е.А."
            };
            Assert.AreEqual(100, test.BatchId);
            Assert.AreEqual("raw_material", test.BatchType);
            Assert.AreEqual("in_progress", test.Status);
            Assert.AreEqual("Петрова Е.А.", test.AnalystName);
        }

        // Проверка добавления результата анализа
        [TestMethod]
        public void Test7()
        {
            var result = new TestResult
            {
                ParameterName = "pH",
                MeasuredValue = 7.2m,
                MinValue = 6.5m,
                MaxValue = 7.5m,
                Unit = "",
                AnalystComment = "В норме"
            };
            result.IsValid = result.MeasuredValue >= result.MinValue && result.MeasuredValue <= result.MaxValue;

            Assert.AreEqual("pH", result.ParameterName);
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("В норме", result.AnalystComment);
        }

        // Проверка дашборда
        [TestMethod]
        public void Test8()
        {
            var dashboard = new DashboardData
            {
                PendingRawMaterials = 8,
                PendingFinishedProducts = 5,
                TestsInProgress = 3,
                CompletedToday = 2,
                BlockedBatches = 1,
                ApprovedBatches = 4
            };
            Assert.AreEqual(8, dashboard.PendingRawMaterials);
            Assert.AreEqual(5, dashboard.PendingFinishedProducts);
            Assert.AreEqual(3, dashboard.TestsInProgress);
        }

        // Проверка истории испытаний
        [TestMethod]
        public void Test9()
        {
            var history = new TestHistoryItem
            {
                Id = 1,
                BatchNumber = "RAW-001",
                SampleType = "Сырье",
                Decision = "approved",
                AnalystName = "Петрова Е.А.",
                CreatedAt = DateTime.Now
            };
            Assert.AreEqual(1, history.Id);
            Assert.AreEqual("RAW-001", history.BatchNumber);
            Assert.AreEqual("approved", history.Decision);
        }

        // Проверка нормативных параметров
        [TestMethod]
        public void Test10()
        {
            var pH = new StandardParameter
            {
                ParameterName = "pH",
                SampleType = "raw_material",
                MinValue = 6.5m,
                MaxValue = 7.5m,
                Unit = ""
            };
            var concentration = new StandardParameter
            {
                ParameterName = "Концентрация",
                SampleType = "finished_product",
                MinValue = 95m,
                MaxValue = 97m,
                Unit = "%"
            };

            Assert.AreEqual(6.5m, pH.MinValue);
            Assert.AreEqual(7.5m, pH.MaxValue);
            Assert.AreEqual(95m, concentration.MinValue);
            Assert.AreEqual(97m, concentration.MaxValue);
            Assert.AreEqual("%", concentration.Unit);
        }
    }
}