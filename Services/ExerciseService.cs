using MudBlazorWebApp1.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MudBlazorWebApp1.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ExerciseService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["RapidApiKey"] ?? "af9a4e120emsh5894bc2a53bdfb0p1c1531jsnfcb82676f796";

            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "exercisedb.p.rapidapi.com");
            _httpClient.BaseAddress = new Uri("https://exercisedb.p.rapidapi.com/");
        }

        public async Task<List<Exercise>> GetExercisesAsync()
        {
            try
            {

                int limit = 0;
                var response = await _httpClient.GetAsync($"exercises?limit={limit}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<Exercise>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var exercises = JsonSerializer.Deserialize<List<Exercise>>(jsonContent, options);
                return exercises ?? new List<Exercise>();
            }
            catch (Exception)
            {
                return new List<Exercise>();
            }
        }
        public async Task<List<string>> GetBodyPartList()
        {
            try
            {

                int limit = 0;
                var response = await _httpClient.GetAsync($"exercises/bodyPartList?limit={limit}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<string>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var bodyPartList = JsonSerializer.Deserialize<List<string>>(jsonContent, options);
                return bodyPartList ?? new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        public async Task<List<string>> GetEquipmentList()
        {
            try
            {

                int limit = 0;
                var response = await _httpClient.GetAsync($"exercises/equipmentList?limit={limit}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<string>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var equipmentList = JsonSerializer.Deserialize<List<string>>(jsonContent, options);
                return equipmentList ?? new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetTargetList()
        {
            try
            {

                int limit = 0;
                var response = await _httpClient.GetAsync($"exercises/targetList?limit={limit}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<string>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var targetList = JsonSerializer.Deserialize<List<string>>(jsonContent, options);
                return targetList ?? new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
        public async Task<Exercise> GetExerciseByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"exercises/exercise/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en la respuesta: {response.StatusCode}, {errorContent}");
                    return new Exercise();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var exercise = JsonSerializer.Deserialize<Exercise>(jsonContent, options);
                return exercise ?? new Exercise();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo ejercicio por id: {ex.Message}");
                return new Exercise();
            }
        }

        public async Task<List<Exercise>> GetExercisesByBodyPartAsync(string bodyPart)
        {
            try
            {
                var response = await _httpClient.GetAsync($"exercises/bodyPart/{bodyPart}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en la respuesta: {response.StatusCode}, {errorContent}");
                    return new List<Exercise>();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var exercises = JsonSerializer.Deserialize<List<Exercise>>(jsonContent, options);
                return exercises ?? new List<Exercise>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo ejercicios por parte del cuerpo: {ex.Message}");
                return new List<Exercise>();
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                Console.WriteLine("Probando conexión a la API...");
                var response = await _httpClient.GetAsync("exercises/bodyPartList");

                var result = response.IsSuccessStatusCode;
                Console.WriteLine($"Resultado de la prueba: {result}, Código: {response.StatusCode}");

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error probando conexión: {ex.Message}");
                return false;
            }
        }
    }
}

