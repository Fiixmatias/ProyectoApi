using Microsoft.EntityFrameworkCore;
using ProyectoApi.Domain.Entities;
using ProyectoApi.Domain.Interfaces;
using ProyectoApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoApi.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }
        //  Agregar Usuario
        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
        }
        //Eliminar Usuario
        public Task DeleteAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            return Task.CompletedTask;
        }
        //Valida si existe el Email
        public Task<bool> ExistEmail(string email)
        {
            return _context.Usuarios.AnyAsync(u=> u.Email == email);
        }
        // Verificador de Email para UpdateUsuario
        public async Task<bool> ExistEmailUpdate(string email, int id)
        {
            return await _context.Usuarios.AnyAsync(u=> u.Email == email && u.Id != id);
        }

        //Obtener Usuario por Email
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u=> u.Email == email);
        }

        //Obtener Usuario por ID
        public async Task<Usuario?> GetByIdAsync(int id)
        {
           return await _context.Usuarios.FindAsync(id);
        }
        // Guardar Cambios
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public  Task UpdateAsync(Usuario usuario, byte[] rowVersion)
        {
            _context.Entry(usuario).Property(u=> u.RowVersion).OriginalValue = rowVersion;
            return Task.CompletedTask;
        }
    }
}
