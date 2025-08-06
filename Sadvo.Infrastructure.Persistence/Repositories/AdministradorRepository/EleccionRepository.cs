using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository
{
    public class EleccionRepository : GenericRepository<Eleccion>, IEleccionesRepository
    {
        private new readonly AppDbContext _context;

        public EleccionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Métodos existentes
        public async Task<Eleccion?> GetByIdWithVotosAsync(int id)
        {
            return await _context.Elecciones
                .Include(e => e.Votos!)
                    .ThenInclude(v => v.PuestoElectivo)
                .Include(e => e.Votos!)
                    .ThenInclude(v => v.Candidato)
                .Include(e => e.Votos!)
                    .ThenInclude(v => v.PartidoPolitico)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> EstaFinalizada(int Id)
        {
            return await _context.Elecciones
                .AnyAsync(c => c.Id == Id && c.Estado == EstadoEleccion.Finalizada);
        }

        public async Task<bool> HayPuestosElectivosActivos()
        {
            return await _context.PuestosElectivos
                .AnyAsync(p => p.Estado == Actividad.Activo);
        }

        public async Task<bool> HayPartidoPolitico(int Id)
        {
            return await _context.PartidosPoliticos // ✅ Corregido
                .CountAsync(c => c.Estado == Actividad.Activo) >= 2;
        }

        public async Task<List<Eleccion>> GetAllWithVotosAsync()
        {
            return await _context.Elecciones
                .Include(e => e.Votos!)
                    .ThenInclude(v => v.PuestoElectivo)
                .Include(e => e.Votos!)
                    .ThenInclude(v => v.PartidoPolitico)
                .ToListAsync();
        }

        // ✅ NUEVOS MÉTODOS NECESARIOS

        // Obtener todas las elecciones ordenadas (más reciente primero, elección activa de primera)
        public async Task<List<Eleccion>> GetEleccionesOrdenadas()
        {
            return await _context.Elecciones
                .OrderByDescending(e => e.Estado == EstadoEleccion.EnProceso ? 1 : 0) // Activas primero
                .ThenByDescending(e => e.FechaRealizacion) // Luego por fecha más reciente
                .ToListAsync();
        }

        // Verificar si hay elección activa
        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Elecciones
                .AnyAsync(e => e.Estado == EstadoEleccion.EnProceso);
        }

        // Obtener elección activa
        public async Task<Eleccion?> GetEleccionActiva()
        {
            return await _context.Elecciones
                .FirstOrDefaultAsync(e => e.Estado == EstadoEleccion.EnProceso);
        }

        // Finalizar elección
        public async Task<bool> FinalizarEleccion(int eleccionId)
        {
            var eleccion = await _context.Elecciones.FindAsync(eleccionId);
            if (eleccion != null && eleccion.Estado == EstadoEleccion.EnProceso)
            {
                eleccion.Estado = EstadoEleccion.Finalizada;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Obtener cantidad de partidos que participaron en una elección
        public async Task<int> GetCantidadPartidosParticipantes(int eleccionId)
        {
            return await _context.Votos
                .Where(v => v.EleccionId == eleccionId)
                .Select(v => v.PartidoPoliticoId)
                .Distinct()
                .CountAsync();
        }

        // Obtener cantidad de puestos disputados en una elección
        public async Task<int> GetCantidadPuestosDisputados(int eleccionId)
        {
            return await _context.Votos
                .Where(v => v.EleccionId == eleccionId)
                .Select(v => v.PuestoElectivoId)
                .Distinct()
                .CountAsync();
        }

        // Validaciones para crear nueva elección
        public async Task<bool> HaySuficientesPartidosActivos()
        {
            return await _context.PartidosPoliticos
                .CountAsync(p => p.Estado == Actividad.Activo) >= 2;
        }

        public async Task<List<PartidoPolitico>> GetPartidosActivos()
        {
            return await _context.PartidosPoliticos
                .Where(p => p.Estado == Actividad.Activo)
                .ToListAsync();
        }

        public async Task<List<PuestoElectivo>> GetPuestosElectivosActivos()
        {
            return await _context.PuestosElectivos
                .Where(p => p.Estado == Actividad.Activo)
                .ToListAsync();
        }

        // Verificar si un partido tiene candidatos para todos los puestos
        public async Task<List<PuestoElectivo>> GetPuestosSinCandidatosPorPartido(int partidoId)
        {
            var puestosActivos = await GetPuestosElectivosActivos();
            var puestosConCandidatos = await _context.AsignacionesCandidatosPuestos
                .Where(acp => acp.PartidoPoliticoId == partidoId)
                .Select(acp => acp.PuestoElectivoId)
                .Distinct()
                .ToListAsync();

            return puestosActivos
                .Where(p => !puestosConCandidatos.Contains(p.Id))
                .ToList();
        }

        // ✅ Obtener votos agrupados para resultados (SIN DTO)
        public async Task<List<Voto>> GetVotosPorEleccion(int eleccionId)
        {
            return await _context.Votos
                .Include(v => v.PuestoElectivo)
                .Include(v => v.Candidato)
                .Include(v => v.PartidoPolitico)
                .Where(v => v.EleccionId == eleccionId)
                .ToListAsync();
        }

        // Crear nueva elección
        public async Task<bool> CrearEleccion(string nombre, DateOnly fechaRealizacion)
        {
            var nuevaEleccion = new Eleccion
            {
                Nombre = nombre,
                FechaRealizacion = fechaRealizacion,
                Estado = EstadoEleccion.EnProceso
            };

            _context.Elecciones.Add(nuevaEleccion);
            await _context.SaveChangesAsync();
            return true;
        }

        // Obtener total de votos por puesto en una elección
        public async Task<int> GetTotalVotosPorPuesto(int eleccionId, int puestoElectivoId)
        {
            return await _context.Votos
                .CountAsync(v => v.EleccionId == eleccionId && v.PuestoElectivoId == puestoElectivoId);
        }

        // Obtener votos por candidato en un puesto específico
        public async Task<int> GetVotosPorCandidato(int eleccionId, int candidatoId, int puestoElectivoId)
        {
            return await _context.Votos
                .CountAsync(v => v.EleccionId == eleccionId &&
                                v.CandidatoId == candidatoId &&
                                v.PuestoElectivoId == puestoElectivoId);
        }
    }
}