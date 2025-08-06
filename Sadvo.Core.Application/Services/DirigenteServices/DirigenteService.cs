using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Domain.Enums;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;

namespace Sadvo.Core.Application.Services.AdministradorServices
{
    public class DirigenteService : IDirigenteService
    {
        private readonly IDirigenteRepository _dirigenteRepository;
        private readonly IEleccionesRepository _eleccionRepository;

        public DirigenteService(
            IDirigenteRepository dirigenteRepository,
            IEleccionesRepository eleccionRepository)
        {
            _dirigenteRepository = dirigenteRepository;
            _eleccionRepository = eleccionRepository;
        }

        public async Task<DirigenteHomeDto> GetDashboardAsync(int usuarioId)
        {
            try
            {
                var partidoInfo = await GetPartidoInfoAsync(usuarioId);
                var indicadores = await GetIndicadoresAsync(usuarioId);

                var hayEleccionActiva = await _eleccionRepository.HayEleccionActiva();
                var eleccionActiva = hayEleccionActiva ? await _eleccionRepository.GetEleccionActiva() : null;

                return new DirigenteHomeDto
                {
                    PartidoInfo = partidoInfo,
                    Indicadores = indicadores,
                    TieneEleccionActiva = hayEleccionActiva,
                    EleccionActivaNombre = eleccionActiva?.Nombre ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener datos del dashboard", ex);
            }
        }

        public async Task<PartidoPoliticoHomeDto> GetPartidoInfoAsync(int usuarioId)
        {
            try
            {
                var partido = await _dirigenteRepository.GetPartidoPoliticoByDirigenteAsync(usuarioId);
                if (partido == null)
                {
                    throw new Exception("Usuario no está asignado a ningún partido político");
                }

                return new PartidoPoliticoHomeDto
                {
                    Id = partido.Id,
                    Nombre = partido.Nombre,
                    Siglas = partido.Siglas,
                    LogoUrl = partido.Logo,
                    Descripcion = partido.Descripcion,
                    EstaActivo = partido.Estado == Actividad.Activo
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener información del partido", ex);
            }
        }

        public async Task<IndicadoresDirigenteDto> GetIndicadoresAsync(int usuarioId)
        {
            try
            {
                // Obtener partido del dirigente
                var partido = await _dirigenteRepository.GetPartidoPoliticoByDirigenteAsync(usuarioId);
                if (partido == null)
                {
                    return new IndicadoresDirigenteDto();
                }

                var partidoId = partido.Id;

                // Obtener todos los indicadores
                var candidatosActivos = await _dirigenteRepository.GetCantidadCandidatosActivosAsync(partidoId);
                var candidatosInactivos = await _dirigenteRepository.GetCantidadCandidatosInactivosAsync(partidoId);
                var alianzas = await _dirigenteRepository.GetCantidadAlianzasAsync(partidoId);
                var solicitudesPendientes = await _dirigenteRepository.GetCantidadSolicitudesPendientesAsync(partidoId);
                var candidatosAsignados = await _dirigenteRepository.GetCantidadCandidatosAsignadosAsync(partidoId);
                var puestosTotales = await _dirigenteRepository.GetCantidadPuestosElectivosTotalesAsync();

                return new IndicadoresDirigenteDto
                {
                    CandidatosActivos = candidatosActivos,
                    CandidatosInactivos = candidatosInactivos,
                    AlianzasPoliticas = alianzas,
                    SolicitudesAlianzasPendientes = solicitudesPendientes,
                    CandidatosAsignadosPuesto = candidatosAsignados,
                    PuestosElectivosTotales = puestosTotales
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener indicadores", ex);
            }
        }
    }
}