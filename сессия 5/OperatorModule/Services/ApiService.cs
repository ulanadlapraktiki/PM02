using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
using OperatorModule.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using User = OperatorModule.Models.User;

namespace OperatorModule.Services
{
    public class ApiService
    {
        private readonly HttpClient _http = new HttpClient();
        private const string BaseUrl = "http://localhost:5127";
        private int? _currentUserId;

        public void SetCurrentUserId(int userId)
        {
            _currentUserId = userId;
            if (_http.DefaultRequestHeaders.Contains("UserId"))
                _http.DefaultRequestHeaders.Remove("UserId");
            _http.DefaultRequestHeaders.Add("UserId", userId.ToString());
        }

        // Логин
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            try
            {
                var data = new { username, password };
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"{BaseUrl}/api/auth/login", content);
                var respJson = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<LoginResponse>(respJson);
                return null;
            }
            catch { return null; }
        }

        // Активные партии
        public async Task<List<ProductionBatch>?> GetRunningBatchesAsync()
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}/api/productionbatches/status/running");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ProductionBatch>>(json);
                }
                return null;
            }
            catch { return null; }
        }

        // Шаги по партии
        public async Task<List<ProductionStep>?> GetStepsByBatchAsync(int batchId)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}/api/productionsteps/batch/{batchId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ProductionStep>>(json);
                }
                return null;
            }
            catch { return null; }
        }

        // Получить партию по ID
        public async Task<ProductionBatch?> GetProductionBatchAsync(int batchId)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}/api/productionbatches/{batchId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ProductionBatch>(json);
                }
                return null;
            }
            catch { return null; }
        }

        // Начать шаг
        public async Task<bool> StartStepAsync(int stepId)
        {
            try
            {
                var response = await _http.PutAsync($"{BaseUrl}/api/productionsteps/{stepId}/start", null);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // Завершить шаг
        public async Task<bool> CompleteStepAsync(CompleteStepRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PutAsync($"{BaseUrl}/api/productionsteps/{request.StepId}/complete", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        // Завершить партию
        public async Task<bool> CompleteBatchAsync(int batchId)
        {
            try
            {
                var response = await _http.PutAsync($"{BaseUrl}/api/productionbatches/{batchId}/complete", null);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"API ошибка: {response.StatusCode}\n{content}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Исключение в CompleteBatchAsync: {ex.Message}");
                return false;
            }
        }

        // Название продукта
        public async Task<string?> GetProductNameByOrderId(int? orderId)
        {
            if (orderId == null) return "Неизвестно";
            try
            {
                var order = await GetAsync<dynamic>($"/api/productionorders/{orderId}");
                if (order?.RecipeId == null) return "Неизвестно";
                var recipe = await GetAsync<dynamic>($"/api/recipes/{order.RecipeId}");
                if (recipe?.ProductId == null) return "Неизвестно";
                var product = await GetAsync<dynamic>($"/api/products/{recipe.ProductId}");
                return product?.Name ?? "Неизвестно";
            }
            catch { return "Неизвестно"; }
        }

        private async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}{url}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch { return default; }
        }

        // Получить пользователя с аватаром
        public async Task<Models.User?> GetUserWithAvatarAsync(int userId)
        {
            return await GetAsync<User>($"/api/users/{userId}");
        }

        // Обновить аватар
        public async Task<bool> UpdateAvatarAsync(int userId, byte[] avatarData)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var byteContent = new ByteArrayContent(avatarData);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(byteContent, "avatar", "avatar.jpg");
                var response = await _http.PostAsync($"{BaseUrl}/api/users/{userId}/avatar", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }
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
}