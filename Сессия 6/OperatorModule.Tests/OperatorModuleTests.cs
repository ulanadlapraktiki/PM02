using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperatorModule.Models;
using System;

namespace OperatorModule.Tests
{
    [TestClass]
    public class OperatorModuleTests
    {
        // Создание активной партии
        [TestMethod]
        public void Test1()
        {
            var batch = new ProductionBatch
            {
                Id = 1,
                BatchNumber = "BATCH-001",
                Status = "running",
                StartTime = DateTime.Now
            };
            Assert.AreEqual("BATCH-001", batch.BatchNumber);
            Assert.AreEqual("running", batch.Status);
            Assert.IsNotNull(batch.StartTime);
        }

        // Создание шага
        [TestMethod]
        public void Test2()
        {
            var step = new ProductionStep
            {
                Id = 1,
                BatchId = 100,
                StepOrder = 1,
                StepName = "Смешивание",
                PlannedTempC = 45,
                PlannedDurationMin = 30,
                PlannedPressureBar = 1.5m
            };
            Assert.AreEqual(1, step.StepOrder);
            Assert.AreEqual("Смешивание", step.StepName);
            Assert.AreEqual(45, step.PlannedTempC);
            Assert.AreEqual(30, step.PlannedDurationMin);
        }

        // Начало шага
        [TestMethod]
        public void Test3()
        {
            var step = new ProductionStep();
            step.StartedAt = DateTime.Now;
            Assert.IsNotNull(step.StartedAt);
        }

        // Завершение шага
        [TestMethod]
        public void Test4()
        {
            var step = new ProductionStep();
            step.CompletedAt = DateTime.Now;
            Assert.IsNotNull(step.CompletedAt);
        }

        // Проверка валидации температуры в норме
        [TestMethod]
        public void Test5()
        {
            decimal plannedTemp = 45;
            decimal actualTemp = 44;
            decimal tolerance = 5;
            bool isValid = Math.Abs(actualTemp - plannedTemp) <= tolerance;
            Assert.IsTrue(isValid);
        }

        // Проверка валидации температуры вне нормы
        [TestMethod]
        public void Test6()
        {
            decimal plannedTemp = 45;
            decimal actualTemp = 52;
            decimal tolerance = 5;
            bool isValid = Math.Abs(actualTemp - plannedTemp) <= tolerance;
            Assert.IsFalse(isValid);
        }

        // Проверка валидации давления в норме
        [TestMethod]
        public void Test7()
        {
            decimal plannedPressure = 1.5m;
            decimal actualPressure = 1.4m;
            decimal tolerance = 0.5m;
            bool isValid = Math.Abs(actualPressure - plannedPressure) <= tolerance;
            Assert.IsTrue(isValid);
        }

        // Проверка валидации давления вне нормы
        [TestMethod]
        public void Test8()
        {
            decimal plannedPressure = 1.5m;
            decimal actualPressure = 2.2m;
            decimal tolerance = 0.5m;
            bool isValid = Math.Abs(actualPressure - plannedPressure) <= tolerance;
            Assert.IsFalse(isValid);
        }

        // Изменение статуса партии
        [TestMethod]
        public void Test9()
        {
            var batch = new ProductionBatch { Status = "pending" };
            batch.Status = "in_progress";
            Assert.AreEqual("in_progress", batch.Status);
        }

        // Создание запроса на завершение шага
        [TestMethod]
        public void Test10()
        {
            var request = new CompleteStepRequest
            {
                StepId = 1,
                ActualTempC = 45,
                ActualDurationMin = 30,
                ActualPressureBar = 1.5m,
                Comment = "Штатное выполнение"
            };
            Assert.AreEqual(1, request.StepId);
            Assert.AreEqual(45, request.ActualTempC);
            Assert.AreEqual("Штатное выполнение", request.Comment);
        }
    }
}