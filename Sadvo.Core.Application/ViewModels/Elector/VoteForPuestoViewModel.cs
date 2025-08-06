
using System.ComponentModel.DataAnnotations;

namespace Sadvo.Core.Application.ViewModels.Elector
{
    public class VoteForPuestoViewModel
    {
        public int PuestoId { get; set; }
        public string PuestoNombre { get; set; } = string.Empty;
        public string PuestoDescripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un candidato.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un candidato válido.")]
        public int SelectedCandidatoId { get; set; }

        public List<CandidatoVotingViewModel> Candidatos { get; set; } = new List<CandidatoVotingViewModel>();
    }
}
