using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class DirigenteHomeDto
    {
        public PartidoPoliticoHomeDto PartidoInfo { get; set; }
        public IndicadoresDirigenteDto Indicadores { get; set; } = new();
        public bool TieneEleccionActiva { get; set; }
        public string EleccionActivaNombre { get; set; } = string.Empty;
    }
}
