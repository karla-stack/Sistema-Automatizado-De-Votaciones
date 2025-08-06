using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class PartidoPoliticoRepository : GenericRepository<PartidoPolitico>, IPartidoPoliticoRepository
    {
        private new readonly AppDbContext _context;
        public PartidoPoliticoRepository(AppDbContext context) : base(context)
        {
            _context = context; 
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>().Where(p => p.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso)
                                 .AnyAsync();
        }
    }
}
