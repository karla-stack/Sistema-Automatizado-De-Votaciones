
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Entities.Dirigente;

namespace Sadvo.Core.Domain.Entities.Elector
{
    public class Voto
    {
        public int Id { get; set; }
        public Ciudadano? Ciudadano { get; set; }
        public int CiudadanoId { get; set; }
        public Eleccion? Eleccion { get; set; }
        public int EleccionId { get; set; }
        public PuestoElectivo? PuestoElectivo { get; set; }
        public int PuestoElectivoId { get; set; }
        public Candidato? Candidato { get; set; }
        public int? CandidatoId { get; set; }
        public PartidoPolitico? PartidoPolitico { get; set; }
        public int PartidoPoliticoId { get; set; }
        public DateTime FechaVoto { get; set; }

    }
}
