
using ProyectoApi.Application.Common;
using ProyectoApi.Application.DTOs;

namespace ProyectoApi.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<string?> LoginAsync(LoginDto dto);
        Task<UsuarioDto?> GetByIdAsync(int id);
        Task<Result<UsuarioDto>> CreateAsync(CreateUsuarioDto dto);
        Task<Result> UpdateAsync(int id,UpdateUsuarioDto dto);
        Task<bool> DeleteAsync(int id);


        
    }
}
