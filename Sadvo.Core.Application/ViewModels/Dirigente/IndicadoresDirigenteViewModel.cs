using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.ViewModels.Dirigente
{
    public class IndicadoresDirigenteViewModel
    {
        public int CandidatosActivos { get; set; }
        public int CandidatosInactivos { get; set; }
        public int AlianzasPoliticas { get; set; }
        public int SolicitudesAlianzasPendientes { get; set; }
        public int CandidatosAsignadosPuesto { get; set; }
        public int PuestosElectivosTotales { get; set; }
        public double PorcentajeCandidatosAsignados { get; set; }

        // Propiedades calculadas
        public int TotalCandidatos => CandidatosActivos + CandidatosInactivos;
        public string PorcentajeCandidatosFormateado => $"{PorcentajeCandidatosAsignados:F1}%";

        // Estados y validaciones
        public bool TieneCandidatos => TotalCandidatos > 0;
        public bool TieneAlianzas => AlianzasPoliticas > 0;
        public bool TieneSolicitudesPendientes => SolicitudesAlianzasPendientes > 0;
        public bool CandidatosCompletos => PuestosElectivosTotales > 0 && CandidatosAsignadosPuesto >= PuestosElectivosTotales;

        // Clases CSS para indicadores
        public string ClaseCandidatosActivos => CandidatosActivos > 0 ? "text-success" : "text-muted";
        public string ClaseCandidatosInactivos => CandidatosInactivos > 0 ? "text-warning" : "text-muted";
        public string ClaseAlianzas => AlianzasPoliticas > 0 ? "text-info" : "text-muted";
        public string ClaseSolicitudes => SolicitudesAlianzasPendientes > 0 ? "text-danger" : "text-muted";
        public string ClaseAsignaciones => CandidatosCompletos ? "text-success" : "text-warning";
        public string ClaseProgressBar => CandidatosCompletos ? "bg-success" : "bg-warning";

        // Iconos para cada indicador
        public string IconoCandidatosActivos => "fas fa-user-check";
        public string IconoCandidatosInactivos => "fas fa-user-times";
        public string IconoAlianzas => "fas fa-handshake";
        public string IconoSolicitudes => "fas fa-clock";
        public string IconoAsignaciones => "fas fa-users-cog";

        // Mensajes descriptivos
        public string MensajeCandidatos => TieneCandidatos
            ? $"Total de {TotalCandidatos} candidatos registrados"
            : "No hay candidatos registrados";

        public string MensajeAlianzas => TieneAlianzas
            ? $"Participando en {AlianzasPoliticas} alianzas"
            : "Sin alianzas políticas";

        public string MensajeSolicitudes => TieneSolicitudesPendientes
            ? $"{SolicitudesAlianzasPendientes} solicitudes por responder"
            : "Sin solicitudes pendientes";

        public string MensajeAsignaciones => CandidatosCompletos
            ? "Todos los puestos tienen candidatos asignados"
            : $"Faltan {PuestosElectivosTotales - CandidatosAsignadosPuesto} asignaciones";
    }
}
