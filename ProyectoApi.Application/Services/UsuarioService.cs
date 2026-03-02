
using Microsoft.EntityFrameworkCore;
using ProyectoApi.Application.Common;
using ProyectoApi.Application.DTOs;
using ProyectoApi.Application.Interfaces;
using ProyectoApi.Domain.Entities;
using ProyectoApi.Domain.Interfaces;


namespace ProyectoApi.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        public UsuarioService(IUsuarioRepository usuarioRepository ,ITokenService tokenService,IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }
        // Crear Usuario
        public async Task<Result<UsuarioDto>> CreateAsync(CreateUsuarioDto dto)
        {
            var existe = await _usuarioRepository.ExistEmail(dto.Email);

            if (existe)
                return Result<UsuarioDto>.Failure("EmailDuplicado");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PassWordHash = _passwordHasher.Hash(dto.Password),
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

                 await _usuarioRepository.AddAsync(usuario);
                 await _usuarioRepository.SaveChangesAsync();

            var usuarioDto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };

            return Result<UsuarioDto>.Success(usuarioDto);
        }
        // Eliminar Usuario
        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if(usuario == null){
                return false;
            }
            await _usuarioRepository.DeleteAsync(usuario);
            await _usuarioRepository.SaveChangesAsync();
            
            return true;
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
        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
               

            if (usuario == null || !usuario.Activo)
                return null;

            bool passwordValida = _passwordHasher.Verify(dto.Password,usuario.PassWordHash);

            if (!passwordValida)
                return null;

            return _tokenService.GenerateToken(usuario);
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
                return Result.Failure("EmailDuplicado");
            }

            usuario.Nombre = dto.Nombre;
            usuario.Email = dto.Email;

             await _usuarioRepository.UpdateAsync(usuario,dto.RowVersion);
            try
            {
                await _usuarioRepository.SaveChangesAsync();
                return Result.Success();

            }catch(DbUpdateConcurrencyException)
            {
                return Result.Failure("ConcurrencyConflict");
            }
           
            
        }
    }
}
