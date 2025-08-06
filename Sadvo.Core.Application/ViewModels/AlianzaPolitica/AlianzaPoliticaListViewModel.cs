
namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class AlianzaPoliticaListViewModel
    {
        public List<SolicitudPendienteViewModel> SolicitudesPendientes { get; set; } = new();
        public List<SolicitudRealizadaViewModel> SolicitudesRealizadas { get; set; } = new();
        public List<AlianzaActivaViewModel> AlianzasActivas { get; set; } = new();
    }
}
