using MySqlConnector;
using MudBlazorWebApp1.Models;
using System.Data;


public class AdministracionService
{
    private readonly MySqlConnection _connection;

    public AdministracionService(MySqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<List<Usuario>> GetUsuariosAsync()
    {
        var usuarios = new List<Usuario>();

        try
        {
            await _connection.OpenAsync();

            var cmd = new MySqlCommand("SELECT * FROM Users", _connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var usuario = new Usuario
                { 
                    Id = reader.GetInt32("Id"),
                    Nombre = reader.IsDBNull("Nombre") ? string.Empty : reader.GetString("Nombre"),
                    Apellido1 = reader.IsDBNull("Apellido1") ? string.Empty : reader.GetString("Apellido1"),
                    Apellido2 = reader.IsDBNull("Apellido2") ? string.Empty : reader.GetString("Apellido2"),
                    Email = reader.IsDBNull("Email") ? string.Empty : reader.GetString("Email"),
                    Password = reader.IsDBNull("Password") ? string.Empty : reader.GetString("Password"),
                    Telefono = reader.IsDBNull("Telefono") ? 0 : reader.GetInt32("Telefono"),
                    Altura = reader.IsDBNull("Altura") ? 0 : reader.GetInt32("Altura"),
                    Peso = reader.IsDBNull("Peso") ? 0.0 : reader.GetDouble("Peso"),
                    FotoBase64 = reader.IsDBNull("FotoBase64") ? string.Empty : reader.GetString("FotoBase64"),
                }
            ;
                usuarios.Add(usuario);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al obtener usuarios: " + ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }

        return usuarios;
    }

    public async Task<bool> EliminarUsuarioAsync(int id)
    {
        try
        {
            await _connection.OpenAsync();

            using var transaction = await _connection.BeginTransactionAsync();

            // 1. Eliminar pesos
            var eliminarPesos = new MySqlCommand("DELETE FROM pesos WHERE idUsuario = @Id", _connection, (MySqlTransaction)transaction);
            eliminarPesos.Parameters.AddWithValue("@Id", id);
            await eliminarPesos.ExecuteNonQueryAsync();

            // 2. Eliminar rutinaejercicios
            var eliminarRutina = new MySqlCommand("DELETE FROM rutinaejercicios WHERE IdUsuario = @Id", _connection, (MySqlTransaction)transaction);
            eliminarRutina.Parameters.AddWithValue("@Id", id);
            await eliminarRutina.ExecuteNonQueryAsync();

            // 3. Eliminar usuario
            var eliminarUsuario = new MySqlCommand("DELETE FROM Users WHERE Id = @Id", _connection, (MySqlTransaction)transaction);
            eliminarUsuario.Parameters.AddWithValue("@Id", id);
            int rowsAffected = await eliminarUsuario.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al eliminar usuario: " + ex.Message);
            return false;
        }
        finally
        {
            await _connection.CloseAsync();
        }
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
                Altura = @Altura
            WHERE Id = @Id", _connection);

            cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            cmd.Parameters.AddWithValue("@Apellido1", usuario.Apellido1 ?? "");
            cmd.Parameters.AddWithValue("@Apellido2", usuario.Apellido2 ?? "");
            cmd.Parameters.AddWithValue("@Email", usuario.Email);
            cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
            cmd.Parameters.AddWithValue("@Altura", usuario.Altura);
            cmd.Parameters.AddWithValue("@Id", usuario.Id);

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
}