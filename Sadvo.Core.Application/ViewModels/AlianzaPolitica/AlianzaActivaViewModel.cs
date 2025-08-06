

namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class AlianzaActivaViewModel
    {
        public int Id { get; set; }
        public string? NombrePartidoAliado { get; set; }
        public string? SiglasPartidoAliado { get; set; }
        public string PartidoCompleto => $"{NombrePartidoAliado} ({SiglasPartidoAliado})";
        public DateOnly FechaAceptacion { get; set; }
        public string FechaFormateada => FechaAceptacion.ToString("dd/MM/yyyy");
    }
}
