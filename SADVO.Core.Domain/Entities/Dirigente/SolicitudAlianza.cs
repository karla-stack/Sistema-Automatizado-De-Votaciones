

using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Dirigente
{
    public class SolicitudAlianza
    {
        public int Id { get; set; }

        public int PartidoSolicitanteId { get; set; }
        public PartidoPolitico PartidoSolicitante { get; set; } = null!;

        public int PartidoReceptorId { get; set; }
        public PartidoPolitico PartidoReceptor { get; set; } = null!;

        public DateOnly FechaSolicitud { get; set; }

        public EstadoSolicitudAlianza Estado { get; set; } // Enum: EnEspera, Aceptada, Rechazada
    }
}
