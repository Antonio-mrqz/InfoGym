using MudBlazorWebApp1.Models;
using MySqlConnector;
using System.Net.Http;
using System.Text.Json;


namespace MudBlazorWebApp1.Services
{
    public class RutinaService
    {
        private readonly MySqlConnection _connection;
        private readonly IExerciseService _exerciseService;

        public RutinaService(MySqlConnection connection, IExerciseService exerciseService)
        {
            _connection = connection;
            _exerciseService = exerciseService;
        }

        // Agregar ejercicio a la rutina
        public async Task<bool> AgregarEjercicioAsync(int usuarioId, string ejercicioId, int diaSemana)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    INSERT INTO rutinaejercicios (IdUsuario, IdEjercicio, Dia)
                    VALUES (@UsuarioId, @EjercicioId, @DiaSemana)
                ", _connection);

                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@EjercicioId", ejercicioId);
                cmd.Parameters.AddWithValue("@DiaSemana", diaSemana);

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

        // Eliminar ejercicio de la rutina (por usuario y ejercicio)
        public async Task<bool> EliminarEjercicioAsync(int idABorrar)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    DELETE FROM rutinaejercicios
                    WHERE Id = @Id
                ", _connection);

                cmd.Parameters.AddWithValue("@Id", idABorrar);

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

        public async Task<List<EjercicioConDia>> GetEjerciciosConDiaUsuarioAsync(int usuarioId)
        {
            var lista = new List<EjercicioConDia>();

            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    SELECT Id, IdEjercicio, Dia 
                    FROM rutinaejercicios 
                    WHERE IdUsuario = @UsuarioId
                ", _connection);

                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                using var reader = await cmd.ExecuteReaderAsync();
                var registros = new List<(int Id, string IdEjercicio, int Dia)>();

                while (await reader.ReadAsync())
                {
                    var id = Convert.ToInt32(reader["Id"]);
                    var idEjercicio = reader["IdEjercicio"].ToString();
                    var dia = Convert.ToInt32(reader["Dia"]);
                    registros.Add((id, idEjercicio, dia));
                }

                await _connection.CloseAsync();

                foreach (var (id, idEjercicio, dia) in registros)
                {
                    var ejercicio = await _exerciseService.GetExerciseByIdAsync(idEjercicio);
                    if (!string.IsNullOrEmpty(ejercicio.Id))
                    {
                        lista.Add(new EjercicioConDia
                        {
                            Id = id,
                            Ejercicio = ejercicio,
                            DiaSemana = dia
                        });
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
