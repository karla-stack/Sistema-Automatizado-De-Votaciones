using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.DirigenteRepository
{
    public class AlianzaPoliticaRepository : GenericRepository<SolicitudAlianza>, IAlianzaPoliticaRepository
    {
        private readonly AppDbContext _appDbContext;

        public AlianzaPoliticaRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> HayEleccionActiva()
        {
            return await _context.Set<Eleccion>()
                .AnyAsync(e => e.Estado == EstadoEleccion.EnProceso);
        }

        // Solicitudes pendientes (recibidas)
        public async Task<List<SolicitudAlianza>> GetSolicitudesPendientesByPartido(int partidoReceptorId)
        {
            return await _context.Set<SolicitudAlianza>()
                .Include(s => s.PartidoSolicitante)
                .Where(s => s.PartidoReceptorId == partidoReceptorId &&
                            s.Estado == EstadoSolicitudAlianza.EnEspera)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<SolicitudAlianza?> GetSolicitudPendienteById(int id)
        {
            return await _context.Set<SolicitudAlianza>()
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .FirstOrDefaultAsync(s => s.Id == id && s.Estado == EstadoSolicitudAlianza.EnEspera);
        }

        public async Task<bool> ActualizarEstadoSolicitud(int solicitudId, EstadoSolicitudAlianza nuevoEstado)
        {
            var solicitud = await _context.Set<SolicitudAlianza>().FindAsync(solicitudId);
            if (solicitud != null && solicitud.Estado == EstadoSolicitudAlianza.EnEspera)
            {
                solicitud.Estado = nuevoEstado;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Solicitudes realizadas (enviadas)
        public async Task<List<SolicitudAlianza>> GetSolicitudesRealizadasByPartido(int partidoSolicitanteId)
        {
            return await _context.Set<SolicitudAlianza>()
                .Include(s => s.PartidoReceptor)
                .Where(s => s.PartidoSolicitanteId == partidoSolicitanteId)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }

        public async Task<SolicitudAlianza?> GetSolicitudRealizadaById(int id)
        {
            return await _context.Set<SolicitudAlianza>()
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> EliminarSolicitud(int solicitudId, int partidoSolicitanteId)
        {
            var solicitud = await _context.Set<SolicitudAlianza>()
                .FirstOrDefaultAsync(s => s.Id == solicitudId &&
                                         s.PartidoSolicitanteId == partidoSolicitanteId);

            if (solicitud != null)
            {
                _context.Set<SolicitudAlianza>().Remove(solicitud);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> PuedeEliminarSolicitud(int solicitudId, int partidoSolicitanteId)
        {
            var solicitud = await _context.Set<SolicitudAlianza>()
                .FirstOrDefaultAsync(s => s.Id == solicitudId &&
                                         s.PartidoSolicitanteId == partidoSolicitanteId);

            return solicitud != null && !await HayEleccionActiva();
        }

        // Crear nuevas solicitudes
        public async Task<List<PartidoPolitico>> GetPartidosDisponiblesParaSolicitud(int partidoSolicitanteId)
        {
            // Partidos que ya tienen solicitudes pendientes (enviadas o recibidas)
            var partidosConSolicitudesPendientes = await _context.Set<SolicitudAlianza>()
                .Where(s => s.Estado == EstadoSolicitudAlianza.EnEspera &&
                            (s.PartidoSolicitanteId == partidoSolicitanteId ||
                             s.PartidoReceptorId == partidoSolicitanteId))
                .Select(s => s.PartidoSolicitanteId == partidoSolicitanteId ?
                             s.PartidoReceptorId : s.PartidoSolicitanteId)
                .ToListAsync();

            // Partidos que ya tienen alianzas aceptadas
            var partidosConAlianzasActivas = await _context.Set<SolicitudAlianza>()
                .Where(s => s.Estado == EstadoSolicitudAlianza.Aceptada &&
                            (s.PartidoSolicitanteId == partidoSolicitanteId ||
                             s.PartidoReceptorId == partidoSolicitanteId))
                .Select(s => s.PartidoSolicitanteId == partidoSolicitanteId ?
                             s.PartidoReceptorId : s.PartidoSolicitanteId)
                .ToListAsync();

            // Combinar todas las exclusiones
            var partidosExcluidos = partidosConSolicitudesPendientes
                .Concat(partidosConAlianzasActivas)
                .Distinct()
                .ToList();

            // Agregar el propio partido a las exclusiones
            partidosExcluidos.Add(partidoSolicitanteId);

            // Obtener partidos disponibles
            return await _context.Set<PartidoPolitico>()
                .Where(p => p.Estado == Actividad.Activo &&
                            !partidosExcluidos.Contains(p.Id))
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<bool> CrearSolicitudAlianza(int partidoSolicitanteId, int partidoReceptorId)
        {
            // Verificar que no exista ya una solicitud pendiente entre estos partidos
            var solicitudExistente = await _context.Set<SolicitudAlianza>()
                .AnyAsync(s => s.Estado == EstadoSolicitudAlianza.EnEspera &&
                              ((s.PartidoSolicitanteId == partidoSolicitanteId && s.PartidoReceptorId == partidoReceptorId) ||
                               (s.PartidoSolicitanteId == partidoReceptorId && s.PartidoReceptorId == partidoSolicitanteId)));

            if (solicitudExistente)
            {
                return false;
            }

            var nuevaSolicitud = new SolicitudAlianza
            {
                PartidoSolicitanteId = partidoSolicitanteId,
                PartidoReceptorId = partidoReceptorId,
                FechaSolicitud = DateOnly.FromDateTime(DateTime.Now),
                Estado = EstadoSolicitudAlianza.EnEspera
            };

            _context.Set<SolicitudAlianza>().Add(nuevaSolicitud);
            await _context.SaveChangesAsync();
            return true;
        }

        // Alianzas activas
        public async Task<List<SolicitudAlianza>> GetAlianzasActivas(int partidoId)
        {
            return await _context.Set<SolicitudAlianza>()
                .Include(s => s.PartidoSolicitante)
                .Include(s => s.PartidoReceptor)
                .Where(s => s.Estado == EstadoSolicitudAlianza.Aceptada &&
                            (s.PartidoSolicitanteId == partidoId || s.PartidoReceptorId == partidoId))
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();
        }
    }
}