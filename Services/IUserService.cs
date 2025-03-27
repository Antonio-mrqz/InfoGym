using MudBlazorWebApp1.Models;

namespace MudBlazorWebApp1.Services
{
    public interface IUserService
    {
        Task<List<Usuario>> GetUsuariosAsync();
        Task<bool> EliminarUsuarioAsync(int id);
        Task<Usuario?> GetUsuarioPorIdAsync(int id);
        Task<bool> ActualizarUsuarioAsync(Usuario usuario);
    }
}
