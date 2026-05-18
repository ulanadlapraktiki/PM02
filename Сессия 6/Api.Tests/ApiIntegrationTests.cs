using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.Tests
{
    [TestClass]
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client = new HttpClient();
        private const string BaseUrl = "http://localhost:5127";

        // Успешная авторизация
        [TestMethod]
        public async Task Test1()
        {
            // Arrange
            var body = new { username = "labo", password = "123" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/api/auth/login", content);
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("userId"));
            Assert.IsTrue(json.Contains("laboratory"));
        }

        // Авторизация с неверным паролем
        [TestMethod]
        public async Task Test2()
        {
            // Arrange
            var body = new { username = "labo", password = "wrong" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/api/auth/login", content);

            // Assert
            Assert.AreEqual(401, (int)response.StatusCode);
        }

        // Получение активных партий
        [TestMethod]
        public async Task Test3()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("UserId", "3");

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/api/productionbatches/status/running");
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("batchNumber"));
        }

        // Получение шагов по партии
        [TestMethod]
        public async Task test4()
        {
            // Arrange
            int batchId = 38;
            _client.DefaultRequestHeaders.Add("UserId", "3");

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/api/productionsteps/batch/{batchId}");
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("stepOrder"));
        }

        // Начало шага
        [TestMethod]
        public async Task Test5()
        {
            // Arrange
            int stepId = 51; // ID существующего шага со статусом pending
            _client.DefaultRequestHeaders.Add("UserId", "3");

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/api/productionsteps/{stepId}/start", null);
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("Шаг начат"));
        }

        // Завершение шага
        [TestMethod]
        public async Task Test6()
        {
            // Arrange
            int stepId = 51;
            var body = new { actualTempC = 45, actualDurationMin = 30, actualPressureBar = 1.5, operatorComment = "Тест" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Add("UserId", "3");

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/api/productionsteps/{stepId}/complete", content);
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("Шаг завершен"));
        }

        // Создание лабораторного испытания
        [TestMethod]
        public async Task Test7()
        {
            // Arrange
            var body = new { batchId = 35, batchType = "finished_product", comment = "Тест" };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            _client.DefaultRequestHeaders.Add("UserId", "10");

            // Act
            var response = await _client.PostAsync($"{BaseUrl}/api/laboratory/tests", content);
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("Испытание создано"));
        }

        // Получение истории испытаний
        [TestMethod]
        public async Task Test8()
        {
            // Arrange
            _client.DefaultRequestHeaders.Add("UserId", "10");

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/api/laboratory/history");
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("batchNumber"));
            Assert.IsTrue(json.Contains("decision"));
        }

        // Получение нормативов
        [TestMethod]
        public async Task Test9()
        {
            // Arrange
            string sampleType = "raw_material";

            // Act
            var response = await _client.GetAsync($"{BaseUrl}/api/laboratory/standards/{sampleType}");
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("pH"));
            Assert.IsTrue(json.Contains("minValue"));
            Assert.IsTrue(json.Contains("maxValue"));
        }

        // Завершение партии
        [TestMethod]
        public async Task Test10()
        {
            // Arrange
            int batchId = 35;
            _client.DefaultRequestHeaders.Add("UserId", "3");

            // Act
            var response = await _client.PutAsync($"{BaseUrl}/api/productionbatches/{batchId}/complete", null);
            var json = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.IsTrue(json.Contains("quality_pending"));
        }
    }
}