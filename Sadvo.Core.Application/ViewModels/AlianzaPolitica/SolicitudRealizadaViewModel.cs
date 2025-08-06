using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class SolicitudRealizadaViewModel
    {
        public int Id { get; set; }
        public string? NombrePartidoReceptor { get; set; }
        public string? SiglasPartidoReceptor { get; set; }
        public string PartidoCompleto => $"{NombrePartidoReceptor} ({SiglasPartidoReceptor})";
        public DateOnly FechaSolicitud { get; set; }
        public string FechaFormateada => FechaSolicitud.ToString("dd/MM/yyyy");
        public EstadoSolicitudAlianza Estado { get; set; }
        public string EstadoTexto => Estado switch
        {
            EstadoSolicitudAlianza.EnEspera => "En espera de respuesta",
            EstadoSolicitudAlianza.Aceptada => "Aceptada",
            EstadoSolicitudAlianza.Rechazada => "Rechazada",
            _ => "Desconocido"
        };
        public string EstadoBadgeClass => Estado switch
        {
            EstadoSolicitudAlianza.EnEspera => "badge-warning",
            EstadoSolicitudAlianza.Aceptada => "badge-success",
            EstadoSolicitudAlianza.Rechazada => "badge-danger",
            _ => "badge-secondary"
        };
    }
}
