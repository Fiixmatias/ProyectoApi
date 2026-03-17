using System.ComponentModel.DataAnnotations;

namespace ProyectoApi.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; }= string.Empty;
        public string PassWordHash { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;



    }
}
 