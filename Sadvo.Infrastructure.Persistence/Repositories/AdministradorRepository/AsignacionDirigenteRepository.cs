using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class AsignacionDirigenteRepository : GenericRepository<AsignacionDirigente>, IAsignacionDirigenteRepository
    {
        private new AppDbContext _context;
        public AsignacionDirigenteRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>().Where(p => p.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso)
                                 .AnyAsync();
        }

        public async Task<bool> HayAsignacionExistente(int usuarioId, int partidoId)
        {
            return await _context.Set<AsignacionDirigente>()
                                  .AnyAsync(a => a.UsuarioId == usuarioId && a.PartidoPoliticoId == partidoId);
        }

        public async Task<List<PartidoPolitico>> ObtenerPartidosActivos()
        {
            return await _context.Set<PartidoPolitico>()
                .Where(p => p.Estado == Core.Domain.Enums.Actividad.Activo)
                .ToListAsync();
        }

        public async Task<List<Usuario>> ObtenerDirigentesDisponibles()
        {
            // Obtener IDs de usuarios que ya tienen asignación
            var usuariosConAsignacion = await _context.Set<AsignacionDirigente>()
                .Select(ad => ad.UsuarioId)
                .ToListAsync();

            // Obtener dirigentes activos que NO están en la lista de asignados
            return await _context.Set<Usuario>()
                .Where(u => u.Estado == Core.Domain.Enums.Actividad.Activo &&
                            u.Rol == Core.Domain.Enums.RolUsuario.Dirigente &&
                            !usuariosConAsignacion.Contains(u.Id))
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.Apellido)
                .ToListAsync();
        }
        public async Task<List<AsignacionDirigente>> ListaAsignacionDirigentesActivos()
        {
            return await _context.Set<AsignacionDirigente>()
                .Include(a => a.PartidoPolitico)
                .Include(a => a.Usuario)
                .Where(a => a.PartidoPolitico != null &&
                            a.PartidoPolitico.Estado == Core.Domain.Enums.Actividad.Activo &&
                            a.Usuario != null &&
                            a.Usuario.Estado == Core.Domain.Enums.Actividad.Activo)
                .ToListAsync();
        }

        public async Task<AsignacionDirigente?> GetByIdAsyncDirigente(int id)
        {
            return await _context.AsignacionesDirigentes
                .Include(x => x.Usuario)
                .Include(x => x.PartidoPolitico)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
