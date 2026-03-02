using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;

namespace ProyectoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [Authorize]
        
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> getUsuario(int id)
        {
            var usuarioDto = await _usuarioService.GetByIdAsync(id);
            if(usuarioDto == null)
            {
                return NotFound("No se encontro el usuario");
            }
            return Ok(usuarioDto);
        }

        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> createUsuario(CreateUsuarioDto dto)
        {
            var result = await _usuarioService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return result.Error switch
                {
                    "EmailDuplicado" => BadRequest("El email ya existe."),
                    _ => StatusCode(500)
                };
            }

            return CreatedAtAction(nameof(getUsuario),
                new { id = result.Value!.Id },
                result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id , UpdateUsuarioDto dto)
        {
            var result = await _usuarioService.UpdateAsync(id, dto);

            if (!result.IsSuccess)
            {
                return result.Error switch
                {
                    "NotFound" => NotFound(),
                    "EmailDuplicado" => BadRequest("El email ya existe."),
                    "ConcurrencyConflict" => Conflict("El registro fue modificado por otro usuario."),
                    _ => StatusCode(500)
                };
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var eliminado = await _usuarioService.DeleteAsync(id);
            if(!eliminado)
            {
                return NotFound();
            }
            return NoContent();
        }



    }
}
