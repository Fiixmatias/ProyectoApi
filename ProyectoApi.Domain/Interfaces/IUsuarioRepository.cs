using ProyectoApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApi.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> ExistEmail(string email);
        Task<bool> ExistEmailUpdate(string email, int id);
        Task<Usuario?> GetByIdAsync(int id);
        Task AddAsync(Usuario usuario);
        Task SaveChangesAsync();
        Task DeleteAsync(Usuario usuario);
        Task<Usuario?> GetByEmailAsync(string email);
        Task UpdateAsync(Usuario usuario, byte[] rowVersion);
    }
}
