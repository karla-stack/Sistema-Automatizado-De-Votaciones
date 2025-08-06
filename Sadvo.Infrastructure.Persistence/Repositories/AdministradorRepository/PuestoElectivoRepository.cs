using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class PuestoElectivoRepository : GenericRepository<PuestoElectivo>, IPuestoElectivoRepository
    {
        private readonly AppDbContext _appDbContext;    
        public PuestoElectivoRepository(AppDbContext context) : base (context)
        {
            _appDbContext = context;
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>().Where(p => p.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso)
                                 .AnyAsync();
        }
    }
}
