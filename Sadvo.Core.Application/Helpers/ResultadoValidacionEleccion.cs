

namespace Sadvo.Core.Application.Helpers
{
    public class ResultadoValidacionEleccion
    {
        public bool EsValida { get; set; }
        public List<string> Mensajes { get; set; } = new();
    }
}
