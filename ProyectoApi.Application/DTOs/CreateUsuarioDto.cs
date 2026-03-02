using System.ComponentModel.DataAnnotations;

namespace ProyectoApi.Application.DTOs
{
    public class CreateUsuarioDto
    {
        [Required  (ErrorMessage="Campo requerido")]
        [StringLength(100,ErrorMessage="Maximo 100 caracteres")]

        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress (ErrorMessage ="Formato de email requerido")]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
