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
        public async Task<bool> ActualizarUsuario(Usuario usuario)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
            UPDATE Users SET
                Nombre = @Nombre,
                Apellido1 = @Apellido1,
                Apellido2 = @Apellido2,
                Email = @Email,
                Telefono = @Telefono,
                Altura = @Altura,
                FotoBase64 = @FotoBase64
            WHERE Id = @Id", _connection);

                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido1", usuario.Apellido1 ?? "");
                cmd.Parameters.AddWithValue("@Apellido2", usuario.Apellido2 ?? "");
                cmd.Parameters.AddWithValue("@Email", usuario.Email);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Altura", usuario.Altura);
                cmd.Parameters.AddWithValue("@Id", usuario.Id);
                cmd.Parameters.AddWithValue("@FotoBase64", usuario.FotoBase64);

                int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                // Puedes loguear ex.Message si lo necesitas
                return false;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<Usuario?> GetUsuarioPorIdAsync(int id)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
            SELECT Id, Nombre, Apellido1, Apellido2, Email, Telefono, Altura, Peso, FotoBase64
            FROM Users
            WHERE Id = @Id", _connection);

                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario
                    {
                        Id = reader.GetInt32("Id"),
                        Nombre = reader["Nombre"]?.ToString(),
                        Apellido1 = reader["Apellido1"]?.ToString(),
                        Apellido2 = reader["Apellido2"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        Telefono = reader["Telefono"] != DBNull.Value ? Convert.ToInt32(reader["Telefono"]) : 0,
                        Altura = reader["Altura"] != DBNull.Value ? Convert.ToInt32(reader["Altura"]) : 0,
                        Peso = reader["Peso"] != DBNull.Value ? Convert.ToDouble(reader["Peso"]) : 0,
                        FotoBase64 = reader["FotoBase64"]?.ToString(),
                    };
                }

                return null;
            }
            catch (Exception)
            {
                // Puedes loguear el error si lo necesitas
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public Task<bool> ActualizarUsuarioAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetFotoBase64Async(int userId)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand("SELECT FotoBase64 FROM Users WHERE Id = @Id", _connection);
                cmd.Parameters.AddWithValue("@Id", userId);

                var result = await cmd.ExecuteScalarAsync();
                return result != DBNull.Value ? result?.ToString() : null;
            }
            catch
            {
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
