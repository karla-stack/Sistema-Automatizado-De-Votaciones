
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.Dtos
{
    public class EleccionDto
    {

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public DateOnly FechaRealizacion { get; set; }
        public EstadoEleccion Estado { get; set; }
    }
}
