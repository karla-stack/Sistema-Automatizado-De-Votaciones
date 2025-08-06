
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Interfaces.DirigenteInterfaces
{
    public interface IAlianzaPoliticaRepository : IGenericRepository<SolicitudAlianza>
    {

        Task<bool> HayEleccionActiva();

        // Solicitudes pendientes (recibidas)
        Task<List<SolicitudAlianza>> GetSolicitudesPendientesByPartido(int partidoReceptorId);
        Task<SolicitudAlianza?> GetSolicitudPendienteById(int id);
        Task<bool> ActualizarEstadoSolicitud(int solicitudId, EstadoSolicitudAlianza nuevoEstado);

        // Solicitudes realizadas (enviadas)
        Task<List<SolicitudAlianza>> GetSolicitudesRealizadasByPartido(int partidoSolicitanteId);
        Task<SolicitudAlianza?> GetSolicitudRealizadaById(int id);
        Task<bool> EliminarSolicitud(int solicitudId, int partidoSolicitanteId);
        Task<bool> PuedeEliminarSolicitud(int solicitudId, int partidoSolicitanteId);

        // Crear nuevas solicitudes
        Task<List<PartidoPolitico>> GetPartidosDisponiblesParaSolicitud(int partidoSolicitanteId);
        Task<bool> CrearSolicitudAlianza(int partidoSolicitanteId, int partidoReceptorId);

        // Alianzas activas
        Task<List<SolicitudAlianza>> GetAlianzasActivas(int partidoId);


    }
}
