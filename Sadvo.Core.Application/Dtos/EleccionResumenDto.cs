

namespace Sadvo.Core.Application.Dtos
{
    public class EleccionResumenDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public DateOnly Fecha { get; set; }
        public int CantPuestos { get; set; }
        public int CantPartidos { get; set; }
        public bool EstaFinalizada { get; set; }
        public bool EstaActiva { get; set; }
    }
}
