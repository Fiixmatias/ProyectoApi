
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProyectoApi.Application.Common;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;
using ProyectoApi.Domain.Entities;
using ProyectoApi.Domain.Interfaces;


namespace ProyectoApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly ILogger<UsuarioService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        public UsuarioService(IUsuarioRepository usuarioRepository ,ITokenService tokenService,IPasswordHasher passwordHasher, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
        // Crear Usuario
        public async Task<Result<UsuarioDto>> CreateAsync(CreateUsuarioDto dto)
        {
            var existe = await _usuarioRepository.ExistEmail(dto.Email);

            if (existe)
            {
                _logger.LogWarning("Intento de crear usuario con Emial duplicado {Email}", dto.Email);
                return Result<UsuarioDto>.Failure("EmailDuplicado");
            }
            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PassWordHash = _passwordHasher.Hash(dto.Password),
                Activo = true,
                FechaCreacion = DateTime.UtcNow,
                RolId = dto.RoleId
            };

                 await _usuarioRepository.AddAsync(usuario);
                 await _usuarioRepository.SaveChangesAsync();
            _logger.LogInformation("Usuario creado exitosamente con id {UsuarioId} y Email {Email}",usuario.Id,usuario.Email);

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };

            return Result<UsuarioDto>.Success(usuarioDto);
        }
        // Eliminar Usuario
      
        public async Task<Result> DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if(usuario == null){
                return Result.Failure("UserNotfound") ;
            }
            await _usuarioRepository.DeleteAsync(usuario);
       
            await _usuarioRepository.SaveChangesAsync();
            _logger.LogInformation("Usuario Eliminado con el id {UsuarioId}", usuario.Id);

            return Result.Success();
        }
        //Obtener Usuario por ID
        public async Task<UsuarioDto?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if(usuario == null)
            {
                return null;
            }
            var usuarioDto = new UsuarioDto 
            
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    RowVersion = usuario.RowVersion

                } ;
            return usuarioDto;
          
        }
        //Login de Usuario
        public async Task<Result<string>> LoginAsync(LoginDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            if(usuario == null)
            {
                _logger.LogWarning("Inteto de login fallido para el Email {Email}", dto.Email);
                return Result<string>.Failure("NotFound");
            }

            if(!usuario.Activo)
            {
                return Result<string>.Failure("NotActive");
            }
           
            bool passwordValida = _passwordHasher.Verify(dto.Password,usuario.PassWordHash);

            if (!passwordValida)
            {
                _logger.LogWarning("Intento fallido de login por contraseña erronea para {Email}", dto.Email);
                return Result<string>.Failure("PassWordError"); ;
            }
            
            return Result<string>.Success(_tokenService.GenerateToken(usuario));
        }

        public async Task<Result> UpdateAsync(int id, UpdateUsuarioDto dto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if(usuario == null)
            {
                return Result.Failure("NotFound");
            }
            var emailExiste = await _usuarioRepository.ExistEmailUpdate(dto.Email,id);
            if(emailExiste)
            {
                _logger.LogWarning("Intento de actualizar Usuario {UsuarioId} con Email duplicado {Email}",id,dto.Email);
                return Result.Failure("EmailDuplicado");
            }

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;

             await _usuarioRepository.UpdateAsync(usuario,dto.RowVersion);
            try
            {
                await _usuarioRepository.SaveChangesAsync();
                _logger.LogInformation("Usuario actualizado con exito con el id {UsuarioId} y el Email {Email}",usuario.Id,usuario.Email);
                return Result.Success();

            }catch(DbUpdateConcurrencyException)
            {
                return Result.Failure("ConcurrencyConflict");
            }
           
            
        }
    }
}
