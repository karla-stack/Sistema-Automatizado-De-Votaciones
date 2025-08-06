using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class CiudadanoRepository : GenericRepository<Ciudadano>, ICiudadanoRepository
    {
        private new readonly AppDbContext _context;
        public CiudadanoRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>().Where(p => p.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso)
                                 .AnyAsync();
        }

        public async Task<bool> HayCedulaExistente(string Identificacion)
        {
            return await _context.Set<Ciudadano>()
                                  .AnyAsync(c => c.Identificacion == Identificacion);
        }

        public async Task<bool> HayCedulaExistente(string Identificacion, int idIgnorar)
        {
            return await _context.Set<Ciudadano>()
                      .AnyAsync(c => c.Identificacion == Identificacion && c.Id != idIgnorar);
        }
    }
}
