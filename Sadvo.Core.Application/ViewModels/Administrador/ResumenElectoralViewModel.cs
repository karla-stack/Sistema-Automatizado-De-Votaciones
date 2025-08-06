using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.ViewModels.Administrador
{
    public class ResumenElectoralViewModel
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = string.Empty;
        public DateOnly FechaEleccion { get; set; }
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatos { get; set; }
        public int TotalVotos { get; set; }
        public string EstadoEleccion { get; set; } = string.Empty;

        // Propiedades para la vista
        public string FechaFormateada => FechaEleccion.ToString("dd/MM/yyyy");
        public string FechaCompleta => FechaEleccion.ToString("dddd, dd 'de' MMMM 'de' yyyy");

        // Formateo de números
        public string CantidadPartidosFormateada => CantidadPartidos.ToString("N0");
        public string CantidadCandidatosFormateada => CantidadCandidatos.ToString("N0");
        public string TotalVotosFormateado => TotalVotos.ToString("N0");

        // Clases CSS para el estado
        public string EstadoClase => EstadoEleccion.ToLower() switch
        {
            "finalizada" => "badge-success",
            "en proceso" => "badge-warning",
            "activa" => "badge-primary",
            _ => "badge-secondary"
        };

        public string EstadoIcono => EstadoEleccion.ToLower() switch
        {
            "finalizada" => "fas fa-check-circle",
            "en proceso" => "fas fa-clock",
            "activa" => "fas fa-play-circle",
            _ => "fas fa-question-circle"
        };

        // Indicadores visuales
        public bool EsEleccionReciente => (DateTime.Now.Date - FechaEleccion.ToDateTime(TimeOnly.MinValue).Date).Days <= 30;
        public bool TieneAltaParticipacion => TotalVotos > 1000; // Ajustar según criterio
        public bool TieneMuchosPartidos => CantidadPartidos >= 5;
        public bool TieneMuchosCandidatos => CantidadCandidatos >= 10;

        // Texto descriptivo
        public string DescripcionParticipacion => TieneAltaParticipacion
            ? "Alta participación"
            : "Participación moderada";

        public string DescripcionCompetencia => TieneMuchosPartidos
            ? "Alta competencia política"
            : "Competencia moderada";

        // Promedio de candidatos por partido (aproximado)
        public double CandidatosPorPartido => CantidadPartidos > 0
            ? Math.Round((double)CantidadCandidatos / CantidadPartidos, 1)
            : 0;

        public string CandidatosPorPartidoTexto => $"{CandidatosPorPartido:F1} candidatos/partido";

        // Porcentaje de participación (si se conoce el total de electores)
        public double? PorcentajeParticipacion { get; set; }
        public string PorcentajeParticipacionTexto => PorcentajeParticipacion.HasValue
            ? $"{PorcentajeParticipacion:F1}%"
            : "N/D";
    }
}
