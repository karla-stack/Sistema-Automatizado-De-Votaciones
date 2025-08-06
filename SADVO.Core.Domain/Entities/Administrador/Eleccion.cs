

using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class Eleccion
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required DateOnly FechaRealizacion { get; set; }
        public EstadoEleccion Estado { get; set; }
        public ICollection<Voto>? Votos { get; set; }
    }
}
