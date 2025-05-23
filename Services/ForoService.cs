using MudBlazorWebApp1.Models;
using MySqlConnector;
using System.Net.Http;
using System.Text.Json;

namespace MudBlazorWebApp1.Services
{
    public class ForoService
    {
        private readonly MySqlConnection _connection;

        public ForoService(MySqlConnection connection, IExerciseService exerciseService)
        {
            _connection = connection;
        }

        public async Task<bool> AgregarComentarioAsync(int usuarioId, string titulo, string texto, int respuestaA)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    INSERT INTO comentariosforo (idUsuario, titulo, texto, respuestaA)
                    VALUES (@UsuarioId, @Titulo, @Texto, @RespuestaA)
                ", _connection);

                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@Titulo", titulo);
                cmd.Parameters.AddWithValue("@Texto", texto);
                cmd.Parameters.AddWithValue("@RespuestaA", respuestaA);

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
        public async Task<List<Comentario>> GetComentarios()
        {
            var lista = new List<Comentario>();

            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    SELECT c.id, c.idUsuario, u.Nombre AS nombreUsuario, c.titulo, c.texto, c.respuestaA
                    FROM comentariosforo c
                    JOIN users u ON c.idUsuario = u.id
                    WHERE c.respuestaA = 0", _connection);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var comentario = new Comentario
                        {
                            Id = reader.GetInt32("id"),
                            IdUsuario = reader.GetInt32("idUsuario"),
                            NombreUsuario = reader.GetString("nombreUsuario"),
                            Titulo = reader.GetString("titulo"),
                            Texto = reader.GetString("texto"),
                            RespuestaA = reader.GetInt32("RespuestaA")
                        };

                        lista.Add(comentario);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ejercicios con día: {ex.Message}");
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    await _connection.CloseAsync();
            }

            return lista;
        }

    }
}
