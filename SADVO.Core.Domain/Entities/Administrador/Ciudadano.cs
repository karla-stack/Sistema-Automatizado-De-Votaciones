
using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class Ciudadano
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public required string Email { get; set; }
        public Actividad Estado { get; set; }
        public required string Identificacion { get; set; }
        public ICollection<Voto>? Votos { get; set; }

    }
}
