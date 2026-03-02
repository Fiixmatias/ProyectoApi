using ProyectoApi.Domain.Entities;

namespace ProyectoApi.Application.Interfaces
{
    public interface ITokenService 
    {
        string GenerateToken(Usuario usuario);
    }
}
