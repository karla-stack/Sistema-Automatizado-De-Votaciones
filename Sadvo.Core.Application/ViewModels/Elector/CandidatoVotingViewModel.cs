
namespace Sadvo.Core.Application.ViewModels.Elector
{
    public class CandidatoVotingViewModel
    {
        public int CandidatoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Foto { get; set; } = string.Empty;
        public string PartidoNombre { get; set; } = string.Empty;
        public string PartidoSiglas { get; set; } = string.Empty;
        public string PartidoLogo { get; set; } = string.Empty;
        public int PartidoId { get; set; }

        public string FullName => $"{Nombre} {Apellido}";
        public string FotoUrl => !string.IsNullOrEmpty(Foto) ? Foto : "/images/default-candidate.png";
        public string LogoUrl => !string.IsNullOrEmpty(PartidoLogo) ? PartidoLogo : "/images/default-party.png";
        public string PartidoDisplay => !string.IsNullOrEmpty(PartidoSiglas) ?
            $"{PartidoNombre} ({PartidoSiglas})" : PartidoNombre;

    }
}
