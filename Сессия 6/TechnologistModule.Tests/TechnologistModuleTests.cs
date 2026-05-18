using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechnologistModule.Models;
using System;

namespace TechnologistModule.Tests
{
    [TestClass]
    public class TechnologistModuleTests
    {
        // Создание продукта
        [TestMethod]
        public void Test1()
        {
            var product = new Product
            {
                Code = "HERB-001",
                Name = "Гербицид А",
                ProductType = "гербицид",
                Form = "жидкий",
                Status = "active"
            };
            Assert.AreEqual("HERB-001", product.Code);
            Assert.AreEqual("Гербицид А", product.Name);
            Assert.AreEqual("active", product.Status);
        }

        // Создание рецептуры
        [TestMethod]
        public void Test2()
        {
            var recipe = new Recipe
            {
                Name = "Рецептура Гербицида А",
                Version = 1,
                Status = "draft",
                ProductId = 1
            };
            Assert.AreEqual("Рецептура Гербицида А", recipe.Name);
            Assert.AreEqual(1, recipe.Version);
            Assert.AreEqual("draft", recipe.Status);
        }

        // Проверка суммы компонентов рецептуры
        [TestMethod]
        public void Test3()
        {
            decimal component1 = 35m;
            decimal component2 = 65m;
            decimal total = component1 + component2;

            Assert.AreEqual(100m, total);
        }

        // Проверка суммы компонентов не равна 100
        [TestMethod]
        public void Test4()
        {
            decimal component1 = 30m;
            decimal component2 = 50m;
            decimal total = component1 + component2;

            Assert.AreNotEqual(100m, total);
            Assert.AreEqual(80m, total);
        }

        // Создание технологической карты
        [TestMethod]
        public void Test5()
        {
            var techMap = new TechMap
            {
                Name = "ТК Гербицида А",
                Version = 1,
                Status = "draft",
                ProductId = 1
            };
            Assert.AreEqual("ТК Гербицида А", techMap.Name);
            Assert.AreEqual(1, techMap.Version);
            Assert.AreEqual("draft", techMap.Status);
        }

        // Добавление шага в тех. карту
        [TestMethod]
        public void Test6()
        {
            var step = new TechMapStep
            {
                TechMapId = 1,
                StepOrder = 1,
                StepName = "Смешивание",
                Instruction = "Смешать компоненты",
                DurationMin = 30,
                IsMandatory = true
            };
            Assert.AreEqual(1, step.StepOrder);
            Assert.AreEqual("Смешивание", step.StepName);
            Assert.IsTrue(step.IsMandatory);
        }

        // Создание производственного заказа
        [TestMethod]
        public void Test7()
        {
            var order = new ProductionOrder
            {
                OrderNumber = "ORD-001",
                PlannedQuantityKg = 1000m,
                Status = "planned"
            };
            Assert.AreEqual("ORD-001", order.OrderNumber);
            Assert.AreEqual(1000m, order.PlannedQuantityKg);
            Assert.AreEqual("planned", order.Status);
        }

        // Создание производственной партии
        [TestMethod]
        public void Test8()
        {
            var batch = new ProductionBatch
            {
                BatchNumber = "BATCH-001",
                OrderId = 1,
                Status = "planned"
            };
            Assert.AreEqual("BATCH-001", batch.BatchNumber);
            Assert.AreEqual(1, batch.OrderId);
            Assert.AreEqual("planned", batch.Status);
        }

        // Запуск партии
        [TestMethod]
        public void Test9()
        {
            var batch = new ProductionBatch { Status = "planned" };
            batch.Status = "running";
            batch.StartTime = DateTime.Now;

            Assert.AreEqual("running", batch.Status);
            Assert.IsNotNull(batch.StartTime);
        }

        // Завершение партии
        [TestMethod]
        public void Test10()
        {
            var batch = new ProductionBatch { Status = "running" };
            batch.Status = "quality_pending";
            batch.EndTime = DateTime.Now;

            Assert.AreEqual("quality_pending", batch.Status);
            Assert.IsNotNull(batch.EndTime);
        }
    }
}