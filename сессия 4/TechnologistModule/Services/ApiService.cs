using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechnologistModule.Models;

namespace TechnologistModule.Services
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

        // ==================== БАЗОВЫЕ МЕТОДЫ ====================
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
            catch { return default; }
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
            catch { return default; }
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
            catch { return default; }
        }

        public async Task<T?> DeleteAsync<T>(string url)
        {
            try
            {
                var response = await _http.DeleteAsync($"{BaseUrl}{url}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                return default;
            }
            catch { return default; }
        }

        // ==================== AUTH ====================
        public Task<LoginResponse?> LoginAsync(string username, string password)
            => PostAsync<LoginResponse>("/api/auth/login", new { username, password });

        public Task<object?> RegisterAsync(string username, string password, string fullName, string role, string email, string phone, string department)
            => PostAsync<object>("/api/auth/register", new { username, password, fullName, role, email, phone, department });

        // ==================== USERS ====================
        public Task<List<User>?> GetUsersAsync() => GetAsync<List<User>>("/api/users");
        public Task<User?> GetUserWithAvatarAsync(int userId) => GetAsync<User>($"/api/users/{userId}");
        public Task<User?> UpdateUserAsync(int id, User user) => PutAsync<User>($"/api/users/{id}", user);
        public Task<object?> DeleteUserAsync(int id) => DeleteAsync<object>($"/api/users/{id}");

        // ==================== AVATAR ====================
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

        // ==================== PRODUCTS ====================
        public Task<List<Product>?> GetProductsAsync() => GetAsync<List<Product>>("/api/products");
        public Task<Product?> GetProductByIdAsync(int id) => GetAsync<Product>($"/api/products/{id}");
        public Task<Product?> CreateProductAsync(Product p) => PostAsync<Product>("/api/products", p);
        public Task<Product?> UpdateProductAsync(int id, Product p) => PutAsync<Product>($"/api/products/{id}", p);
        public Task<object?> DeleteProductAsync(int id) => DeleteAsync<object>($"/api/products/{id}");

        // ==================== RECIPES ====================
        public Task<List<Recipe>?> GetRecipesAsync() => GetAsync<List<Recipe>>("/api/recipes");
        public Task<Recipe?> GetRecipeByIdAsync(int id) => GetAsync<Recipe>($"/api/recipes/{id}");
        public Task<Recipe?> CreateRecipeAsync(Recipe r) => PostAsync<Recipe>("/api/recipes", r);
        public Task<Recipe?> UpdateRecipeAsync(int id, Recipe r) => PutAsync<Recipe>($"/api/recipes/{id}", r);
        public Task<object?> DeleteRecipeAsync(int id) => DeleteAsync<object>($"/api/recipes/{id}");
        public Task<object?> ApproveRecipeAsync(int id) => PutAsync<object>($"/api/recipes/{id}/approve", null);
        public Task<List<Recipe>?> GetRecipesByProductAsync(int productId)
            => GetAsync<List<Recipe>>($"/api/recipes/product/{productId}");

        // ==================== TECH MAPS ====================
        public Task<List<TechMap>?> GetTechMapsAsync() => GetAsync<List<TechMap>>("/api/techmaps");
        public Task<TechMap?> GetTechMapByIdAsync(int id) => GetAsync<TechMap>($"/api/techmaps/{id}");
        public Task<TechMap?> CreateTechMapAsync(TechMap t) => PostAsync<TechMap>("/api/techmaps", t);
        public Task<TechMap?> UpdateTechMapAsync(int id, TechMap t) => PutAsync<TechMap>($"/api/techmaps/{id}", t);
        public Task<object?> DeleteTechMapAsync(int id) => DeleteAsync<object>($"/api/techmaps/{id}");
        public Task<object?> ApproveTechMapAsync(int id, int approvedBy)
            => PutAsync<object>($"/api/techmaps/{id}/approve", approvedBy);
        public Task<List<TechMap>?> GetTechMapsByProductAsync(int productId)
            => GetAsync<List<TechMap>>($"/api/techmaps/product/{productId}");

        // ==================== TECH MAP STEPS ====================
        public Task<List<TechMapStep>?> GetTechMapStepsAsync(int techMapId)
            => GetAsync<List<TechMapStep>>($"/api/techmapsteps/techmap/{techMapId}");
        public Task<TechMapStep?> CreateTechMapStepAsync(TechMapStep step)
            => PostAsync<TechMapStep>("/api/techmapsteps", step);
        public Task<TechMapStep?> UpdateTechMapStepAsync(int id, TechMapStep step)
            => PutAsync<TechMapStep>($"/api/techmapsteps/{id}", step);
        public Task<object?> DeleteTechMapStepAsync(int id)
            => DeleteAsync<object>($"/api/techmapsteps/{id}");
        public Task<List<TechMapStep>?> GetTechMapStepsByEquipmentAsync(int equipmentId)
            => GetAsync<List<TechMapStep>>($"/api/techmapsteps/equipment/{equipmentId}");

        // ==================== PRODUCTION ORDERS ====================
        public Task<List<ProductionOrder>?> GetOrdersAsync() => GetAsync<List<ProductionOrder>>("/api/productionorders");
        public Task<ProductionOrder?> GetOrderByIdAsync(int id) => GetAsync<ProductionOrder>($"/api/productionorders/{id}");
        public Task<ProductionOrder?> CreateOrderAsync(ProductionOrder o) => PostAsync<ProductionOrder>("/api/productionorders", o);
        public Task<ProductionOrder?> UpdateOrderAsync(int id, ProductionOrder o)
            => PutAsync<ProductionOrder>($"/api/productionorders/{id}", o);
        public Task<object?> DeleteOrderAsync(int id) => DeleteAsync<object>($"/api/productionorders/{id}");
        public Task<object?> StartOrderAsync(int id) => PutAsync<object>($"/api/productionorders/{id}/start", null);
        public Task<object?> CompleteOrderAsync(int id) => PutAsync<object>($"/api/productionorders/{id}/complete", null);
        public Task<List<ProductionOrder>?> GetOrdersByStatusAsync(string status)
            => GetAsync<List<ProductionOrder>>($"/api/productionorders/status/{status}");

        // ==================== PRODUCTION BATCHES ====================
        public Task<List<ProductionBatch>?> GetBatchesAsync() => GetAsync<List<ProductionBatch>>("/api/productionbatches");
        public Task<ProductionBatch?> GetBatchByIdAsync(int id) => GetAsync<ProductionBatch>($"/api/productionbatches/{id}");
        public Task<ProductionBatch?> CreateBatchAsync(ProductionBatch b) => PostAsync<ProductionBatch>("/api/productionbatches", b);
        public Task<ProductionBatch?> UpdateBatchAsync(int id, ProductionBatch b)
            => PutAsync<ProductionBatch>($"/api/productionbatches/{id}", b);
        public Task<object?> StartBatchAsync(int id) => PutAsync<object>($"/api/productionbatches/{id}/start", null);
        public Task<object?> CompleteBatchAsync(int id) => PutAsync<object>($"/api/productionbatches/{id}/complete", null);
        public Task<List<ProductionBatch>?> GetBatchesByStatusAsync(string status)
            => GetAsync<List<ProductionBatch>>($"/api/productionbatches/status/{status}");
        public Task<List<ProductionBatch>?> GetBatchesByOrderAsync(int orderId)
            => GetAsync<List<ProductionBatch>>($"/api/productionbatches/order/{orderId}");
        public Task<object?> DeleteBatchAsync(int id)
            => DeleteAsync<object>($"/api/productionbatches/{id}");

        // ==================== PRODUCTION STEPS ====================
        public Task<List<ProductionStep>?> GetStepsAsync() => GetAsync<List<ProductionStep>>("/api/productionsteps");
        public Task<ProductionStep?> GetStepByIdAsync(int id) => GetAsync<ProductionStep>($"/api/productionsteps/{id}");
        public Task<List<ProductionStep>?> GetStepsByBatchAsync(int batchId)
            => GetAsync<List<ProductionStep>>($"/api/productionsteps/batch/{batchId}");
        public Task<ProductionStep?> GetCurrentStepAsync(int batchId)
            => GetAsync<ProductionStep>($"/api/productionsteps/batch/{batchId}/current");
        public Task<object?> StartStepAsync(int id) => PutAsync<object>($"/api/productionsteps/{id}/start", null);
        public Task<object?> CompleteStepAsync(int id, StepCompletionRequest request)
            => PutAsync<object>($"/api/productionsteps/{id}/complete", request);

        // ==================== EXTRUDER PROGRAMS ====================
        public Task<List<ExtruderProgram>?> GetExtruderProgramsAsync() => GetAsync<List<ExtruderProgram>>("/api/extruderprograms");
        public Task<ExtruderProgram?> GetExtruderProgramByIdAsync(int id)
            => GetAsync<ExtruderProgram>($"/api/extruderprograms/{id}");
        public Task<ExtruderProgram?> CreateExtruderProgramAsync(ExtruderProgram p)
            => PostAsync<ExtruderProgram>("/api/extruderprograms", p);
        public Task<ExtruderProgram?> UpdateExtruderProgramAsync(int id, ExtruderProgram p)
            => PutAsync<ExtruderProgram>($"/api/extruderprograms/{id}", p);
        public Task<object?> DeleteExtruderProgramAsync(int id)
            => DeleteAsync<object>($"/api/extruderprograms/{id}");
        public Task<object?> ActivateExtruderProgramAsync(int id)
            => PutAsync<object>($"/api/extruderprograms/{id}/activate", null);
        public Task<List<ExtruderProgramZone>?> GetExtruderProgramZonesAsync(int programId)
            => GetAsync<List<ExtruderProgramZone>>($"/api/extruderprograms/{programId}/zones");
        public Task<ExtruderProgramZone?> AddExtruderZoneAsync(int programId, ExtruderProgramZone zone)
            => PostAsync<ExtruderProgramZone>($"/api/extruderprograms/{programId}/zones", zone);
        public Task<object?> DeleteExtruderZoneAsync(int id)
            => DeleteAsync<object>($"/api/extruderprogramzones/{id}");


        // ==================== DEVIATIONS ====================
        public Task<List<Deviation>?> GetDeviationsAsync() => GetAsync<List<Deviation>>("/api/deviations");
        public Task<Deviation?> GetDeviationByIdAsync(int id) => GetAsync<Deviation>($"/api/deviations/{id}");
        public Task<Deviation?> CreateDeviationAsync(Deviation d) => PostAsync<Deviation>("/api/deviations", d);
        public Task<object?> ResolveDeviationAsync(int id, string comment)
            => PutAsync<object>($"/api/deviations/{id}/resolve", comment);
        public Task<List<Deviation>?> GetDeviationsByBatchAsync(int batchId)
            => GetAsync<List<Deviation>>($"/api/deviations/batch/{batchId}");
        public Task<List<Deviation>?> GetUnresolvedDeviationsAsync()
            => GetAsync<List<Deviation>>("/api/deviations/unresolved");
        public Task<object?> DeleteDeviationAsync(int id)
            => DeleteAsync<object>($"/api/deviations/{id}");

        // ==================== QUALITY CONTROL ====================
        public Task<List<QualityControl>?> GetQualityControlsAsync() => GetAsync<List<QualityControl>>("/api/qualitycontrols");
        public Task<QualityControl?> GetQualityControlByIdAsync(int id)
            => GetAsync<QualityControl>($"/api/qualitycontrols/{id}");
        public Task<QualityControl?> CreateQualityControlAsync(QualityControl qc)
            => PostAsync<QualityControl>("/api/qualitycontrols", qc);
        public Task<object?> SetQualityDecisionAsync(int id, string decision, string comment)
            => PutAsync<object>($"/api/qualitycontrols/{id}/decision", new { decision, comment });
        public Task<List<QualityControl>?> GetQualityControlsByBatchAsync(int batchId)
            => GetAsync<List<QualityControl>>($"/api/qualitycontrols/batch/{batchId}");

        // ==================== REPORTS ====================
        public Task<DashboardData?> GetDashboardAsync() => GetAsync<DashboardData>("/api/reports/dashboard");
        public Task<List<BatchReportItem>?> GetBatchReportAsync() => GetAsync<List<BatchReportItem>>("/api/reports/batches");

        // ==================== EQUIPMENT ====================
        public Task<List<Equipment>?> GetEquipmentAsync() => GetAsync<List<Equipment>>("/api/equipment");
        public Task<Equipment?> GetEquipmentByIdAsync(int id) => GetAsync<Equipment>($"/api/equipment/{id}");
        public Task<Equipment?> CreateEquipmentAsync(Equipment e) => PostAsync<Equipment>("/api/equipment", e);
        public Task<Equipment?> UpdateEquipmentAsync(int id, Equipment e)
            => PutAsync<Equipment>($"/api/equipment/{id}", e);
        public Task<Equipment?> UpdateEquipmentStatusAsync(int id, string status)
            => PutAsync<Equipment>($"/api/equipment/{id}/status", status);
        public Task<List<Equipment>?> GetEquipmentByStatusAsync(string status)
            => GetAsync<List<Equipment>>($"/api/equipment/status/{status}");

        // ==================== MATERIALS ====================
        public Task<List<TechnologistModule.Models.MaterialDto>?> GetMaterialsAsync()
            => GetAsync<List<TechnologistModule.Models.MaterialDto>>("/api/materials");
        public Task<TechnologistModule.Models.MaterialDto?> GetMaterialByIdAsync(int id)
            => GetAsync<TechnologistModule.Models.MaterialDto>($"/api/materials/{id}");
        public Task<TechnologistModule.Models.MaterialDto?> CreateMaterialAsync(TechnologistModule.Models.MaterialDto m)
            => PostAsync<TechnologistModule.Models.MaterialDto>("/api/materials", m);
        public Task<TechnologistModule.Models.MaterialDto?> UpdateMaterialAsync(int id, TechnologistModule.Models.MaterialDto m)
            => PutAsync<TechnologistModule.Models.MaterialDto>($"/api/materials/{id}", m);
        public Task<object?> DeleteMaterialAsync(int id)
            => DeleteAsync<object>($"/api/materials/{id}");

        // ==================== RECIPE COMPONENTS ====================
        public Task<List<RecipeComponent>?> GetRecipeComponentsAsync(int recipeId)
            => GetAsync<List<RecipeComponent>>($"/api/recipecomponents/recipe/{recipeId}");
        public Task<RecipeComponent?> AddRecipeComponentAsync(RecipeComponent rc)
            => PostAsync<RecipeComponent>("/api/recipecomponents", rc);
        public Task<RecipeComponent?> UpdateRecipeComponentAsync(int id, RecipeComponent rc)
            => PutAsync<RecipeComponent>($"/api/recipecomponents/{id}", rc);
        public Task<object?> DeleteRecipeComponentAsync(int id)
            => DeleteAsync<object>($"/api/recipecomponents/{id}");
        public Task<object?> GetRecipeComponentsSumAsync(int recipeId)
            => GetAsync<object>($"/api/recipecomponents/recipe/{recipeId}/sum");
    }

    public class StepCompletionRequest
    {
        public decimal? ActualTempC { get; set; }
        public int? ActualDurationMin { get; set; }
        public decimal? ActualPressureBar { get; set; }
        public string OperatorComment { get; set; } = string.Empty;
    }
}