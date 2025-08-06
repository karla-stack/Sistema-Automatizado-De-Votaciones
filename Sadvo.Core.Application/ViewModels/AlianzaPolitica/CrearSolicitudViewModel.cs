

using Sadvo.Core.Application.Dtos;

namespace Sadvo.Core.Application.ViewModels.AlianzaPolitica
{
    public class CrearSolicitudViewModel
    {
        public int PartidoReceptorId { get; set; }
        public List<PartidoDisponibleDto> PartidosDisponibles { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool HasPartidosDisponibles => PartidosDisponibles != null && PartidosDisponibles.Any();
    }
}
