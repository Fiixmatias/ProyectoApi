
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;


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
            if(token == null)
            {
                return Unauthorized("Credenciales invalidas");
            }
            return Ok(new {token});
        }
    }
}
