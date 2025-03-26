using MySqlConnector;
using BCrypt.Net;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using MudBlazorWebApp1.Models;
namespace MudBlazorWebApp1.Services
{
    public class AuthService
    {
        private readonly MySqlConnection _connection;
        private readonly IJSRuntime _jsRuntime;

        public AuthService(MySqlConnection connection, IJSRuntime jsRuntime)
        {
            _connection = connection;
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> Register(string username, string email, string password)
        {
            try
            {
                await _connection.OpenAsync();

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var cmd = new MySqlCommand(@"
                INSERT INTO Users (Nombre, Email, Password) 
                VALUES (@Nombre, @Email, @Password)", _connection);

                cmd.Parameters.AddWithValue("@Nombre", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", passwordHash);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<Usuario> Login(string email, string password)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                SELECT *
                FROM Users 
                WHERE Email = @Email", _connection);

                cmd.Parameters.AddWithValue("@Email", email);

                var reader = await cmd.ExecuteReaderAsync();

                if (reader.Read())
                {
                    string storedPassword = reader["Password"].ToString();
                    if (BCrypt.Net.BCrypt.Verify(password, storedPassword))
                    {
                        // Crear un objeto Usuario con los datos de la base de datos
                        var usuario = new Usuario
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Email = reader["Email"].ToString(),
                        };
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "user", JsonConvert.SerializeObject(usuario));

                        return usuario;  // Retornar el objeto Usuario
                    }
                }

                return null;  // Si no se encuentra el usuario o la contraseña no coincide
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        public async Task<Usuario> GetCurrentUser()
        {
            var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "user");

            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonConvert.DeserializeObject<Usuario>(userJson);
            }

            return null;  // Si no hay un usuario guardado en localStorage
        }
        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "user");
        }

    }
}
