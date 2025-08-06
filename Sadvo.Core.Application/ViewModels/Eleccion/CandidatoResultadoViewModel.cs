

namespace Sadvo.Core.Application.ViewModels.Eleccion
{
    public class CandidatoResultadoViewModel
    {
        public int CandidatoId { get; set; }
        public string NombreCandidato { get; set; } = "";
        public string Partido { get; set; } = "";
        public string SiglasPartido { get; set; } = "";
        public int CantidadVotos { get; set; }
        public double Porcentaje { get; set; }
        public string PorcentajeFormateado => $"{Porcentaje:F2}%";
    }
}
