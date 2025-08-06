using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Infrastructure.Persistence.Contexts;
using SADVO.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Core.Application.Helpers;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuariosRepository
    {
        private AppDbContext _appDbContext; 
        public UsuarioRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;    
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>().Where(p => p.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso)
                                 .AnyAsync();
        }

        public async Task<bool> HayNombreExistenteAsync(string nombreUsuario)
        {
            return await _context.Set<Usuario>()
                                 .AnyAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<bool> HayNombreExistenteAsync(string nombreUsuario, int idIgnorar)
        {
            return await _context.Set<Usuario>()
                                 .AnyAsync(u => u.NombreUsuario == nombreUsuario && u.Id != idIgnorar);
        }

        public async Task<Usuario?> LoginAsync(string username, string password)
        {
            string passwordEncrypt = PasswoardEncryptation.EncryptPassword(password);

            Usuario? usuario = await _context.Set<Usuario>().FirstOrDefaultAsync(u => u.NombreUsuario == username && u.Contrasena == passwordEncrypt);

            return usuario;
        }

        public async Task<bool> HayAdmin()
        {
            return await _context.Set<Usuario>()
                                 .AnyAsync(p => p.Rol == Core.Domain.Enums.RolUsuario.Administrador);
        }

        public async Task<Usuario?> GetByUsernameWithAsignacionAsync(string username)
        {
            return await _context.Usuarios
                .Include(u => u.AsignacionDirigente)
                .ThenInclude(ad => ad.PartidoPolitico)
                .FirstOrDefaultAsync(u => u.NombreUsuario == username);
        }
    }
}
