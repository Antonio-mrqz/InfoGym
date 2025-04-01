using MySqlConnector;
using MudBlazorWebApp1.Models;

namespace MudBlazorWebApp1.Services
{
    public class PesoService
    {
        private readonly MySqlConnection _connection;

        public PesoService(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<bool> GuardarPeso(int idUsuario, Double peso)
        {
            try
            {
                await _connection.OpenAsync();
                var cmd = new MySqlCommand("INSERT INTO pesos (idUsuario, peso) VALUES (@idUsuario, @peso)", _connection);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@peso", peso);
                await cmd.ExecuteNonQueryAsync();
                cmd = new MySqlCommand("UPDATE users SET Peso = @peso  WHERE ID = @idUsuario", _connection);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                cmd.Parameters.AddWithValue("@peso", peso);
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        public async Task<List<Peso>> ObtenerHistorialPesos(int idUsuario)
        {
            var pesos = new List<Peso>();
            try
            {
                await _connection.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM pesos WHERE idUsuario = @idUsuario ORDER BY fecha ASC", _connection);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                var reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    pesos.Add(new Peso
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        IdUsuario = Convert.ToInt32(reader["idUsuario"]),
                        Valor = Convert.ToInt32(reader["peso"]),
                        Fecha = Convert.ToDateTime(reader["fecha"])
                    });
                }
            }
            catch { }
            finally
            {
                await _connection.CloseAsync();
            }
            return pesos;
        }
    }
}