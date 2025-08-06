
namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class SolicitudPendienteViewModel
    {
        public int Id { get; set; }
        public string? NombrePartidoSolicitante { get; set; }
        public string? SiglasPartidoSolicitante { get; set; }
        public string PartidoCompleto => $"{NombrePartidoSolicitante} ({SiglasPartidoSolicitante})";
        public DateOnly FechaSolicitud { get; set; }
        public string FechaFormateada => FechaSolicitud.ToString("dd/MM/yyyy");
    }
}
