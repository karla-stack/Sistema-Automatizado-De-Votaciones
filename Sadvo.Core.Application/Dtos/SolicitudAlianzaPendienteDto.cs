using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class SolicitudAlianzaPendienteDto
    {
        public int Id { get; set; }
        public int PartidoSolicitanteId { get; set; }
        public string? NombrePartidoSolicitante { get; set; }
        public string? SiglasPartidoSolicitante { get; set; }
        public DateOnly FechaSolicitud { get; set; }
        public EstadoSolicitudAlianza Estado { get; set; }
    }
}
