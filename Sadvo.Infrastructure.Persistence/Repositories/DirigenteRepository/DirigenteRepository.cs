// Infrastructure/Repositories/AdminRepositories/DirigenteRepository.cs
using Microsoft.EntityFrameworkCore;
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;

namespace Sadvo.Infrastructure.Persistence.Repositories.AdminRepositories
{
    public class DirigenteRepository : IDirigenteRepository
    {
        private readonly AppDbContext _context;

        public DirigenteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PartidoPolitico?> GetPartidoPoliticoByDirigenteAsync(int usuarioId)
        {
            try
            {
                var asignacion = await _context.AsignacionesDirigentes
                    .Include(a => a.PartidoPolitico)
                    .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);

                return asignacion?.PartidoPolitico;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener partido político del dirigente", ex);
            }
        }

        public async Task<List<Candidato>> GetCandidatosByPartidoAsync(int partidoId)
        {
            try
            {
                return await _context.Candidatos
                    .Include(c => c.PartidoPolitico)
                    .Where(c => c.PartidoPoliticoId == partidoId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener candidatos del partido", ex);
            }
        }

        public async Task<int> GetCantidadCandidatosActivosAsync(int partidoId)
        {
            try
            {
                return await _context.Candidatos
                    .CountAsync(c => c.PartidoPoliticoId == partidoId && c.Estado == Actividad.Activo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar candidatos activos", ex);
            }
        }

        public async Task<int> GetCantidadCandidatosInactivosAsync(int partidoId)
        {
            try
            {
                return await _context.Candidatos
                    .CountAsync(c => c.PartidoPoliticoId == partidoId && c.Estado == Actividad.Inactivo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar candidatos inactivos", ex);
            }
        }

        public async Task<List<AlianzaPolitica>> GetAlianzasByPartidoAsync(int partidoId)
        {
            try
            {
                return await _context.AlianzasPoliticas
                    .Include(a => a.PartidoA)
                    .Include(a => a.PartidoB)
                    .Where(a => (a.PartidoAId == partidoId || a.PartidoBId == partidoId)
                               && a.Estado == Actividad.Activo)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener alianzas del partido", ex);
            }
        }

        public async Task<int> GetCantidadAlianzasAsync(int partidoId)
        {
            try
            {
                return await _context.AlianzasPoliticas
                    .CountAsync(a => (a.PartidoAId == partidoId || a.PartidoBId == partidoId)
                               && a.Estado == Actividad.Activo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar alianzas", ex);
            }
        }

        public async Task<List<SolicitudAlianza>> GetSolicitudesAlianzasPendientesAsync(int partidoId)
        {
            try
            {
                return await _context.SolicitudAlianzas
                    .Include(s => s.PartidoSolicitante)
                    .Include(s => s.PartidoReceptor)
                    .Where(s => s.PartidoReceptorId == partidoId
                               && s.Estado == EstadoSolicitudAlianza.EnEspera)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener solicitudes pendientes", ex);
            }
        }

        public async Task<int> GetCantidadSolicitudesPendientesAsync(int partidoId)
        {
            try
            {
                return await _context.SolicitudAlianzas
                    .CountAsync(s => s.PartidoReceptorId == partidoId
                               && s.Estado == EstadoSolicitudAlianza.EnEspera);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar solicitudes pendientes", ex);
            }
        }

        public async Task<List<AsignacionCandidatoPuesto>> GetCandidatosAsignadosAsync(int partidoId)
        {
            try
            {
                return await _context.AsignacionesCandidatosPuestos
                    .Include(a => a.Candidato)
                    .Include(a => a.PuestoElectivo)
                    .Include(a => a.PartidoPolitico)
                    .Where(a => a.PartidoPoliticoId == partidoId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener candidatos asignados", ex);
            }
        }

        public async Task<int> GetCantidadCandidatosAsignadosAsync(int partidoId)
        {
            try
            {
                return await _context.AsignacionesCandidatosPuestos
                    .CountAsync(a => a.PartidoPoliticoId == partidoId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar candidatos asignados", ex);
            }
        }

        // MÉTODO CORREGIDO - Tu PuestoElectivo usa Estado (enum Actividad)
        public async Task<int> GetCantidadPuestosElectivosTotalesAsync()
        {
            try
            {
                return await _context.PuestosElectivos
                    .CountAsync(p => p.Estado == Actividad.Activo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar puestos electivos", ex);
            }
        }

        public async Task<bool> EsDirigenteDelPartidoAsync(int usuarioId, int partidoId)
        {
            try
            {
                return await _context.AsignacionesDirigentes
                    .AnyAsync(a => a.UsuarioId == usuarioId && a.PartidoPoliticoId == partidoId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar dirigente del partido", ex);
            }
        }
    }
}