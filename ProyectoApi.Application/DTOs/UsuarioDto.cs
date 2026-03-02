namespace ProyectoApi.Application.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
