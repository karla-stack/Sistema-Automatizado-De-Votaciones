

namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class AlianzaPoliticaIndexViewModel
    {
        public List<SolicitudPendienteViewModel> SolicitudesPendientes { get; set; } = new();
        public List<SolicitudRealizadaViewModel> SolicitudesRealizadas { get; set; } = new();
        public List<AlianzaActivaViewModel> AlianzasActivas { get; set; } = new();
        public bool EleccionActiva { get; set; }
        public bool SinPartido { get; set; }

        // Propiedades de ayuda para la vista
        public bool TieneSolicitudesPendientes => SolicitudesPendientes != null && SolicitudesPendientes.Any();
        public bool TieneSolicitudesRealizadas => SolicitudesRealizadas != null && SolicitudesRealizadas.Any();
        public bool TieneAlianzasActivas => AlianzasActivas != null && AlianzasActivas.Any();
        public int TotalSolicitudesPendientes => SolicitudesPendientes?.Count ?? 0;
        public int TotalSolicitudesRealizadas => SolicitudesRealizadas?.Count ?? 0;
        public int TotalAlianzasActivas => AlianzasActivas?.Count ?? 0;
    }
}
