namespace MudBlazorWebApp1.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Telefono { get; set; }
        public int Altura { get; set; }
        public Double Peso { get; set; }

        public string LetraNombre => !string.IsNullOrEmpty(Nombre) ? Nombre.Substring(0, 1).ToUpper() : string.Empty;
    }
}