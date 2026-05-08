using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TechnologistModule
{
    public class ApiService
    {
        private readonly HttpClient _http = new HttpClient();
        private const string BaseUrl = "http://localhost:5127";

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _http.GetAsync($"{BaseUrl}{url}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка API: {ex.Message}");
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"{BaseUrl}{url}", content);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка API: {ex.Message}");
            }
        }

        public async Task<T?> PutAsync<T>(string url, object? data = null)
        {
            try
            {
                var content = data == null ? null : new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await _http.PutAsync($"{BaseUrl}{url}", content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка API: {ex.Message}");
            }
        }

        // Auth
        public Task<LoginResponse?> LoginAsync(string username, string password)
            => PostAsync<LoginResponse>("/api/auth/login", new { username, password });

        // Products
        public Task<List<Product>?> GetProductsAsync() => GetAsync<List<Product>>("/api/products");
        public Task<Product?> CreateProductAsync(Product p) => PostAsync<Product>("/api/products", p);

        // Recipes
        public Task<List<Recipe>?> GetRecipesAsync() => GetAsync<List<Recipe>>("/api/recipes");
        public Task<Recipe?> CreateRecipeAsync(Recipe r) => PostAsync<Recipe>("/api/recipes", r);
        public Task ApproveRecipeAsync(int id) => PutAsync<object>($"/api/recipes/{id}/approve", null);

        // Orders
        public Task<List<ProductionOrder>?> GetOrdersAsync() => GetAsync<List<ProductionOrder>>("/api/productionorders");
        public Task<ProductionOrder?> CreateOrderAsync(ProductionOrder o) => PostAsync<ProductionOrder>("/api/productionorders", o);
        public Task StartOrderAsync(int id) => PutAsync<object>($"/api/productionorders/{id}/start", null);

        // Batches
        public Task<List<ProductionBatch>?> GetBatchesAsync() => GetAsync<List<ProductionBatch>>("/api/productionbatches");
        public Task<ProductionBatch?> CreateBatchAsync(ProductionBatch b) => PostAsync<ProductionBatch>("/api/productionbatches", b);
        public Task StartBatchAsync(int id) => PutAsync<object>($"/api/productionbatches/{id}/start", null);
        public Task CompleteBatchAsync(int id) => PutAsync<object>($"/api/productionbatches/{id}/complete", null);

        // Deviations
        public Task<List<Deviation>?> GetDeviationsAsync() => GetAsync<List<Deviation>>("/api/deviations");
        public Task ResolveDeviationAsync(int id, string comment) => PutAsync<object>($"/api/deviations/{id}/resolve", comment);

        // Reports
        public Task<DashboardData?> GetDashboardAsync() => GetAsync<DashboardData>("/api/reports/dashboard");

        public Task<List<BatchReportItem>?> GetBatchReportAsync()
        {
            return GetAsync<List<BatchReportItem>>("/api/reports/batches");
        }
    }
}