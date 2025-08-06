
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;

namespace Sadvo.Core.Application.Interfaces.DirigenteInterfaces
{
    public interface IAlianzaPoliticaService
    {
        Task<ResultadoOperacion> ValidacionEleccionActiva();

        Task<List<SolicitudAlianzaPendienteDto>> GetSolicitudesPendientes();

        Task<bool> AceptarSolicitud(int solicitudId);

        Task<bool> RechazarSolicitud(int solicitudId);

        Task<List<SolicitudAlianzaRealizadaDto>> GetSolicitudesRealizadas();
        Task<bool> EliminarSolicitudRealizada(int solicitudId);
        Task<bool> PuedeEliminarSolicitud(int solicitudId);

        Task<List<PartidoDisponibleDto>> GetPartidosDisponiblesParaSolicitud();

        Task<bool> CrearSolicitudAlianza(int partidoReceptorId);

        Task<List<AlianzaActivaDto>> GetAlianzasActivas();
    }
}
