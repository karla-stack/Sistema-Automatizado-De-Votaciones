using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.DirigenteRepository
{
    public class AsignarCandidatoRepository : GenericRepository<AsignacionCandidatoPuesto>, IAsignacionCandidatoRepository
    {
        private readonly AppDbContext _appDbContext;
        public AsignarCandidatoRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _appDbContext.Set<Eleccion>()
                .Where(p => p.Estado == EstadoEleccion.EnProceso)
                .AnyAsync();
        }

        public async Task<List<AsignacionCandidatoPuesto>> ListaAsignacionCandidatoPuesto(int partidoDirigenteId)
        {
            return await _context.Set<AsignacionCandidatoPuesto>()
                .Include(a => a.Candidato)
                .Include(a => a.PuestoElectivo)
                .Include(a => a.PartidoPolitico)
                .Where(a => a.Candidato != null &&
                            a.Candidato.Estado == Core.Domain.Enums.Actividad.Activo &&
                            a.PuestoElectivo != null &&
                            a.PuestoElectivo.Estado == Core.Domain.Enums.Actividad.Activo &&
                            a.PartidoPoliticoId == partidoDirigenteId)
                .OrderBy(a => a.Candidato.Nombre)
                .ThenBy(a => a.Candidato.Apellido)
                .ToListAsync();
        }

        public async Task<List<Candidato>> ObtenerCandidatosDisponibles(int partidoDirigenteId)
        {
            // 1. Obtener partidos aliados con alianzas aceptadas
            var partidosAliados = await _context.Set<SolicitudAlianza>()
                .Where(a => (a.PartidoSolicitanteId == partidoDirigenteId || a.PartidoReceptorId == partidoDirigenteId) &&
                            a.Estado == EstadoSolicitudAlianza.Aceptada)
                .Select(a => a.PartidoSolicitanteId == partidoDirigenteId ? a.PartidoReceptorId : a.PartidoSolicitanteId)
                .ToListAsync();

            // 2. Lista de todos los partidos permitidos (propio + aliados)
            var todosLosPartidos = new List<int> { partidoDirigenteId };
            todosLosPartidos.AddRange(partidosAliados);

            // 3. Candidatos ya asignados del partido del dirigente (NO pueden repetirse)
            var candidatosAsignadosPartidoPropio = await _context.Set<AsignacionCandidatoPuesto>()
                .Where(a => a.PartidoPoliticoId == partidoDirigenteId)
                .Select(a => a.CandidatoId)
                .ToListAsync();

            // 4. Obtener asignaciones de candidatos de partidos aliados con sus puestos
            var asignacionesPartidosAliados = await _context.Set<AsignacionCandidatoPuesto>()
                .Include(a => a.PuestoElectivo)
                .Where(a => partidosAliados.Contains(a.PartidoPoliticoId))
                .ToListAsync();

            // 5. Obtener todos los candidatos activos de los partidos permitidos
            var todosCandidatos = await _context.Set<Candidato>()
                .Where(c => c.Estado == Core.Domain.Enums.Actividad.Activo &&
                            todosLosPartidos.Contains(c.PartidoPoliticoId))
                .ToListAsync();

            // 6. Aplicar reglas de filtrado
            var candidatosDisponibles = new List<Candidato>();

            foreach (var candidato in todosCandidatos)
            {
                // Regla 1: Si es del partido propio y ya está asignado, NO incluir
                if (candidato.PartidoPoliticoId == partidoDirigenteId &&
                    candidatosAsignadosPartidoPropio.Contains(candidato.Id))
                {
                    continue;
                }

                // Regla 2: Si es de partido aliado, validar reglas especiales
                if (partidosAliados.Contains(candidato.PartidoPoliticoId))
                {
                    var asignacionEnPartidoOriginal = asignacionesPartidosAliados
                        .FirstOrDefault(a => a.CandidatoId == candidato.Id);

                    if (asignacionEnPartidoOriginal != null)
                    {
                        // El candidato aliado ya tiene asignación en su partido original
                        // Verificar si ya está asignado al mismo puesto en nuestro partido
                        var yaAsignadoMismoPuesto = await _context.Set<AsignacionCandidatoPuesto>()
                            .AnyAsync(a => a.CandidatoId == candidato.Id &&
                                      a.PartidoPoliticoId == partidoDirigenteId &&
                                      a.PuestoElectivoId == asignacionEnPartidoOriginal.PuestoElectivoId);

                        if (yaAsignadoMismoPuesto)
                        {
                            continue; // Ya está asignado al mismo puesto
                        }

                        // Solo puede aspirar al mismo puesto que tiene en su partido original
                        // Verificar si el puesto está disponible en nuestro partido
                        var puestoDisponibleEnNuestroPartido = !await _context.Set<AsignacionCandidatoPuesto>()
                            .AnyAsync(a => a.PartidoPoliticoId == partidoDirigenteId &&
                                      a.PuestoElectivoId == asignacionEnPartidoOriginal.PuestoElectivoId);

                        if (!puestoDisponibleEnNuestroPartido)
                        {
                            continue; // El puesto ya está ocupado en nuestro partido
                        }
                    }
                }

                candidatosDisponibles.Add(candidato);
            }

            return candidatosDisponibles
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellido)
                .ToList();
        }

        public async Task<List<PuestoElectivo>> ObtenerPuestosDisponibles(int partidoDirigenteId)
        {
            // Obtener puestos ya ocupados por el partido del dirigente
            var puestosOcupados = await _context.Set<AsignacionCandidatoPuesto>()
                .Where(a => a.PartidoPoliticoId == partidoDirigenteId)
                .Select(a => a.PuestoElectivoId)
                .ToListAsync();

            // Obtener puestos disponibles
            return await _context.Set<PuestoElectivo>()
                .Where(p => p.Estado == Core.Domain.Enums.Actividad.Activo &&
                            !puestosOcupados.Contains(p.Id))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // Método para validar candidato específico para un puesto específico
        public async Task<bool> ValidarCandidatoParaPuesto(int candidatoId, int puestoElectivoId, int partidoDirigenteId)
        {
            var candidato = await _context.Set<Candidato>().FindAsync(candidatoId);
            if (candidato == null) return false;

            // Si es del partido propio, verificar que no esté ya asignado
            if (candidato.PartidoPoliticoId == partidoDirigenteId)
            {
                var yaAsignado = await _context.Set<AsignacionCandidatoPuesto>()
                    .AnyAsync(a => a.CandidatoId == candidatoId && a.PartidoPoliticoId == partidoDirigenteId);

                return !yaAsignado; // Solo válido si no está asignado
            }

            // Si es de partido aliado, verificar reglas especiales
            var esPartidoAliado = await _context.Set<SolicitudAlianza>()
                .AnyAsync(a => (a.PartidoSolicitanteId == partidoDirigenteId || a.PartidoReceptorId == partidoDirigenteId) &&
                              (a.PartidoSolicitanteId == candidato.PartidoPoliticoId || a.PartidoReceptorId == candidato.PartidoPoliticoId) &&
                              a.Estado == EstadoSolicitudAlianza.Aceptada);

            if (!esPartidoAliado) return false;

            // Verificar si el candidato tiene asignación en su partido original
            var asignacionOriginal = await _context.Set<AsignacionCandidatoPuesto>()
                .FirstOrDefaultAsync(a => a.CandidatoId == candidatoId &&
                                         a.PartidoPoliticoId == candidato.PartidoPoliticoId);

            if (asignacionOriginal != null)
            {
                // Solo puede aspirar al mismo puesto
                return asignacionOriginal.PuestoElectivoId == puestoElectivoId;
            }

            // Si no tiene asignación en su partido original, puede aspirar a cualquier puesto disponible
            return true;
        }

        public async Task<List<PuestoElectivo>> ObtenerPuestosDisponiblesParaCandidato(int candidatoId, int partidoDirigenteId)
        {
            var candidato = await _context.Set<Candidato>().FindAsync(candidatoId);
            if (candidato == null) return new List<PuestoElectivo>();

            var puestosDisponibles = await ObtenerPuestosDisponibles(partidoDirigenteId);

            // Si es de partido aliado, filtrar por su puesto actual
            if (candidato.PartidoPoliticoId != partidoDirigenteId)
            {
                var asignacionOriginal = await _context.Set<AsignacionCandidatoPuesto>()
                    .FirstOrDefaultAsync(a => a.CandidatoId == candidatoId &&
                                             a.PartidoPoliticoId == candidato.PartidoPoliticoId);

                if (asignacionOriginal != null)
                {
                    // Solo puede aspirar al mismo puesto que tiene en su partido original
                    puestosDisponibles = puestosDisponibles
                        .Where(p => p.Id == asignacionOriginal.PuestoElectivoId)
                        .ToList();
                }
            }

            return puestosDisponibles;
        }

        public override async Task<AsignacionCandidatoPuesto?> GetByIdAsync(int id)
        {
            return await _context.Set<AsignacionCandidatoPuesto>()
                .Include(a => a.Candidato)
                .Include(a => a.PuestoElectivo)
                .Include(a => a.PartidoPolitico)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}