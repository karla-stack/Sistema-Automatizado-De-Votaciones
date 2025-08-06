
using Sadvo.Core.Application.Dtos;

namespace Sadvo.Core.Application.ViewModels.AsignacionCandidato
{
    public class AsignacionCandidatoForm
    {
        public int IdCandidato { get; set; }
        public int IdPuestoElectivo { get; set; }

        public List<CandidatoDto>? Candidatos { get; set; }
        public List<PuestoElectivoDto>? Puestos { get; set; }

        public string? Mensaje { get; set; }
    }
}
