
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Domain.Entities.Dirigente
{
    public class AsignacionCandidatoPuesto
    {
        public int Id { get; set; }

        public Candidato? Candidato { get; set; }

        public int CandidatoId { get; set; }

        public PuestoElectivo? PuestoElectivo { get; set; }

        public int PuestoElectivoId { get; set; }

        public PartidoPolitico? PartidoPolitico { get; set; }
        public int PartidoPoliticoId { get; set; }
    }
}
