
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;
using ProyectoApi.Application.Common;

namespace ProyectoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _usuarioService.LoginAsync(dto);
            if(!token.IsSuccess)
            {
                return token.Error switch
                {
                    "NotFound" => NotFound("Email no registrado"),
                    "NotActive" => Unauthorized("Usuario no activo"),
                    "PassWordError" => Unauthorized("Contrseña incorrecta"),
                    _ => BadRequest("Error desconocido")
                };
            }
            return Ok(new {token.Value});
        }
    }
}
