using MudBlazorWebApp1.Models;

namespace MudBlazorWebApp1.Services
{
    public interface IExerciseService
    {
        Task<List<Exercise>> GetExercisesAsync();
        Task<Exercise> GetExerciseByIdAsync(string id);
        Task<List<Exercise>> GetExercisesByBodyPartAsync(string bodyPart);
        Task<bool> TestConnectionAsync();
    }
}
