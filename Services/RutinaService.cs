using MySqlConnector;

namespace MudBlazorWebApp1.Services
{
    public class RutinaService
    {
        private readonly MySqlConnection _connection;

        public RutinaService(MySqlConnection connection)
        {
            _connection = connection;
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
        public async Task<bool> EliminarEjercicioAsync(int usuarioId, string ejercicioId)
        {
            try
            {
                await _connection.OpenAsync();

                var cmd = new MySqlCommand(@"
                    DELETE FROM rutinaejercicios
                    WHERE IdUsuario = @UsuarioId AND IdEjercicio = @EjercicioId
                ", _connection);

                cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@EjercicioId", ejercicioId);

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
    }
}
