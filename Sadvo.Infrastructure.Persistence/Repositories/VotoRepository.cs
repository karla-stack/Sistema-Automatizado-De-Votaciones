using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Interfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.ElectorRepositories
{
    public class VotoRepository : IVotoRepository
    {
        private readonly AppDbContext _context;

        public VotoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasVotedForPuestoAsync(int ciudadanoId, int puestoId)
        {
            // Obtener la elección activa
            var activeElection = await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso);

            if (activeElection == null) return false;

            return await _context.Votos
                .AnyAsync(v => v.CiudadanoId == ciudadanoId &&
                              v.PuestoElectivoId == puestoId &&
                              v.EleccionId == activeElection.Id);
        }

        public async Task<bool> HasVotedInActiveElectionAsync(int ciudadanoId)
        {
            var activeElection = await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso);

            if (activeElection == null) return false;

            return await _context.Votos
                .AnyAsync(v => v.CiudadanoId == ciudadanoId && v.EleccionId == activeElection.Id);
        }

        public async Task<List<Voto>> GetVotedPuestosByElectorAsync(int ciudadanoId)
        {
            var activeElection = await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso);

            if (activeElection == null) return new List<Voto>();

            return await _context.Votos
                .Where(v => v.CiudadanoId == ciudadanoId && v.EleccionId == activeElection.Id)
                .Include(v => v.PuestoElectivo)
                .Include(v => v.Candidato)
                .Include(v => v.PartidoPolitico)
                .ToListAsync();
        }

        public async Task<Voto> CreateVoteAsync(Voto voto)
        {
            voto.FechaVoto = DateTime.UtcNow;
            _context.Votos.Add(voto);
            await _context.SaveChangesAsync();
            return voto;
        }

        public async Task<int> GetVotedPuestosCountAsync(int ciudadanoId)
        {
            var activeElection = await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == Core.Domain.Enums.EstadoEleccion.EnProceso);

            if (activeElection == null) return 0;

            return await _context.Votos
                .Where(v => v.CiudadanoId == ciudadanoId && v.EleccionId == activeElection.Id)
                .Select(v => v.PuestoElectivoId)
                .Distinct()
                .CountAsync();
        }

        public async Task<List<Candidato>> GetCandidatosByPuestoAsync(int puestoId)
        {
            // Obtener candidatos que tienen asignaciones para este puesto específico
            return await _context.AsignacionesCandidatosPuestos
                .Where(acp => acp.PuestoElectivoId == puestoId)
                .Include(acp => acp.Candidato)
                    .ThenInclude(c => c.PartidoPolitico)
                .Where(acp => acp.Candidato != null && acp.Candidato.Estado == Core.Domain.Enums.Actividad.Activo)
                .Select(acp => acp.Candidato!)
                .Distinct() // Para evitar duplicados si un candidato tiene múltiples asignaciones
                .ToListAsync();
        }
    }
}   