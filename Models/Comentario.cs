namespace MudBlazorWebApp1.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }
        public int RespuestaA { get; set; }
    }
}
