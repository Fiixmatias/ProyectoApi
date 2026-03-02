using System.ComponentModel.DataAnnotations;

namespace ProyectoApi.Application.DTOs
{
    public class UpdateUsuarioDto
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(100, ErrorMessage = "Maximo 100 caracteres")]
        [MinLength(1,ErrorMessage = "Debe tener minimo 1 caracter")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress(ErrorMessage = "Formato de email requerido")]
        public string Email { get; set; }
        public byte[] RowVersion { get; set; }

    }
}
