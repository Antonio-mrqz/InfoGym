using MudBlazorWebApp1.Models;
using MySqlConnector;

namespace MudBlazorWebApp1.Services
{
    public class UserService : IUserService
    {
        private readonly MySqlConnection _connection;

        public UserService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            var usuarios = new List<Usuario>();
            await _connection.OpenAsync();

            var cmd = new MySqlCommand("SELECT * FROM Users", _connection);
            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new Usuario
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Nombre = reader["Nombre"].ToString(),
                    Apellido1 = reader["Apellido1"].ToString(),
                    Apellido2 = reader["Apellido2"].ToString(),
                    Email = reader["Email"].ToString(),
                    Telefono = Convert.ToInt32(reader["Telefono"]),
                    Altura = Convert.ToInt32(reader["Altura"]),
                    Peso = Convert.ToInt32(reader["Peso"])
                });
            }

            await _connection.CloseAsync();
            return usuarios;
        }

        public async Task<bool> EliminarUsuarioAsync(int id)
        {
            await _connection.OpenAsync();

            var cmd = new MySqlCommand("DELETE FROM Users WHERE Id = @Id", _connection);
            cmd.Parameters.AddWithValue("@Id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            await _connection.CloseAsync();
            return rowsAffected > 0;
        }

        public Task<Usuario?> GetUsuarioPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ActualizarUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
