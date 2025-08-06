

using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Elector;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Dirigente
{
    public class Candidato
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Apellido { get; set; }
        public string? Foto { get; set; }
        public Actividad Estado { get; set; }
        public PartidoPolitico? PartidoPolitico { get; set; }
        public int PartidoPoliticoId { get; set; }
        public ICollection<AsignacionCandidatoPuesto>? PuestosAsignados { get; set; }
        public ICollection<Voto>? Votos { get; set; }
    }
}
