using LaboratoryModule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LaboratoryModule.Services
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

        public async Task<T?> GetAsync<T>(string url)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GET Error: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"{BaseUrl}{url}", content);
                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(respJson);
                }
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POST Error: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string url, object? data = null)
        {
            try
            {
                var json = data == null ? null : JsonConvert.SerializeObject(data);
                var content = json == null ? null : new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PutAsync($"{BaseUrl}{url}", content);
                if (response.IsSuccessStatusCode)
                {
                    var respJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(respJson);
                }
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PUT Error: {ex.Message}");
                return default;
            }
        }

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

        // ==================== AUTH ====================
        public Task<LoginResponse?> LoginAsync(string username, string password)
            => PostAsync<LoginResponse>("/api/auth/login", new { username, password });

        public Task<object?> RegisterAsync(string username, string password, string fullName, string role, string email, string phone, string department)
            => PostAsync<object>("/api/auth/register", new { username, password, fullName, role, email, phone, department });

        // ==================== USERS ====================
        public Task<User?> GetUserWithAvatarAsync(int userId)
            => GetAsync<User>($"/api/users/{userId}");

        // ==================== ПАРТИИ СЫРЬЯ ====================
        public Task<List<RawMaterialDto>?> GetRawMaterialBatchesAsync()
            => GetAsync<List<RawMaterialDto>>("/api/laboratory/raw-materials");

        // ==================== ГОТОВАЯ ПРОДУКЦИЯ ====================
        public async Task<List<ProductionBatch>?> GetFinishedProductBatchesForControlAsync()
        {
            return await GetAsync<List<ProductionBatch>>("/api/productionbatches/status/quality_pending");
        }

        // ==================== НОРМАТИВЫ ====================
        public Task<List<StandardParameter>?> GetStandardParametersAsync(string sampleType)
            => GetAsync<List<StandardParameter>>($"/api/laboratory/standards/{sampleType}");

        // ==================== ОТЧЕТЫ ====================
        public Task<DashboardData?> GetDashboardAsync()
            => GetAsync<DashboardData>("/api/laboratory/dashboard");

        // ==================== ИСТОРИЯ ====================
        public Task<List<TestHistoryItem>?> GetTestHistoryAsync()
            => GetAsync<List<TestHistoryItem>>("/api/laboratory/history");

        // ==================== СОЗДАНИЕ ИСПЫТАНИЯ (старый метод) ====================
        public Task<object?> CreateTestAsync(CreateTestRequest request)
            => PostAsync<object>("/api/laboratory/tests", request);

        // ==================== СОЗДАНИЕ ИСПЫТАНИЯ (НОВЫЙ МЕТОД С ВОЗВРАТОМ ID) ====================
        public async Task<int?> CreateTestAndGetIdAsync(CreateTestRequest request)
        {
            try
            {
                var response = await _http.PostAsJsonAsync($"{BaseUrl}/api/laboratory/tests", request);
                var respJson = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(respJson);
                    if (result != null && result.ContainsKey("id"))
                    {
                        return Convert.ToInt32(result["id"]);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        // ==================== ДОБАВЛЕНИЕ РЕЗУЛЬТАТА ====================
        public Task<object?> AddTestResultAsync(int testId, AddTestResultRequest request)
            => PostAsync<object>($"/api/laboratory/tests/{testId}/results", request);

        // ==================== ЗАВЕРШЕНИЕ ИСПЫТАНИЯ ====================
        public Task<object?> CompleteTestAsync(int testId, CompleteTestRequest request)
            => PutAsync<object>($"/api/laboratory/tests/{testId}/complete", request);

        // ==================== РЕЗУЛЬТАТЫ ИСПЫТАНИЙ ====================
        public Task<List<TestResult>?> GetTestResultsAsync(int testId)
            => GetAsync<List<TestResult>>($"/api/laboratory/tests/{testId}/results");
    }
}