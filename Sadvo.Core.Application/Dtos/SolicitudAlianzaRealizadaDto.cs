using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class SolicitudAlianzaRealizadaDto
    {
        public int Id { get; set; }
        public int PartidoReceptorId { get; set; }
        public string? NombrePartidoReceptor { get; set; }
        public string? SiglasPartidoReceptor { get; set; }
        public DateOnly FechaSolicitud { get; set; }
        public EstadoSolicitudAlianza Estado { get; set; }
        public string EstadoTexto => Estado switch
        {
            EstadoSolicitudAlianza.EnEspera => "En espera de respuesta",
            EstadoSolicitudAlianza.Aceptada => "Aceptada",
            EstadoSolicitudAlianza.Rechazada => "Rechazada",
            _ => "Desconocido"
        };
    }
}
