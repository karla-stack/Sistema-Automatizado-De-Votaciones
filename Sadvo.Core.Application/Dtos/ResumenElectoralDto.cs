using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class ResumenElectoralDto
    {
        public int EleccionId { get; set; }
        public string NombreEleccion { get; set; } = string.Empty;
        public DateOnly FechaEleccion { get; set; }
        public int CantidadPartidos { get; set; }
        public int CantidadCandidatos { get; set; }
        public int TotalVotos { get; set; }
        public string EstadoEleccion { get; set; } = string.Empty;
    }
}
