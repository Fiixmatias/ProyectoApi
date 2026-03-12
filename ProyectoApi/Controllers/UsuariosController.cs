using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoApi.Application.Common;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;
using ProyectoApi.Authorization;
using System.Security.Claims;

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

        [HttpGet("{id}")]
        [Authorize(Policy = Policies.ADMIN_ONLY)]
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
        [Authorize(Policy = Policies.ADMIN_ONLY)]
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
        [Authorize(Policy = Policies.USER_OR_ADMIN)]
        public async Task<IActionResult> UpdateUsuario(int id , UpdateUsuarioDto dto)
        {
            var userId = 0;
            try
            {
                userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            }
            catch
            {
                return Result.Failure("InvalidUserId").Error switch
                {
                    "InvalidUserId" => BadRequest("El ID del usuario no es válido."),
                    _ => StatusCode(400)
                };
            }
            
            if(userId != id)
            {
                return Result.Failure("InvalidUserId").Error switch
                {
                    "InvalidUserId" => BadRequest("No tienes permiso para actualizar este usuario."),
                    _ => StatusCode(400)
                };
            }
            

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
        [Authorize(Policy = Policies.ADMIN_ONLY)]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var result = await _usuarioService.DeleteAsync(id);
            if(!result.IsSuccess)
            {
                return NotFound();
            }
            return NoContent();
        }



    }
}
