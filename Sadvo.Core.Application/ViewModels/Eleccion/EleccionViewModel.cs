
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class EleccionViewModel
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public DateOnly FechaRealizacion { get; set; }
        public EstadoEleccion Estado { get; set; }
        public string? Mensaje { get; set; }
        public List<string> MensajesValidacion { get; set; } = new();
        public bool PuedeCrearEleccion { get; set; } = true;
    }
}
