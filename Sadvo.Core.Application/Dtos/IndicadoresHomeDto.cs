using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class IndicadoresDirigenteDto
    {
        public int CandidatosActivos { get; set; }
        public int CandidatosInactivos { get; set; }
        public int AlianzasPoliticas { get; set; }
        public int SolicitudesAlianzasPendientes { get; set; }
        public int CandidatosAsignadosPuesto { get; set; }
        public int PuestosElectivosTotales { get; set; }

        // Propiedades calculadas
        public int TotalCandidatos => CandidatosActivos + CandidatosInactivos;
        public double PorcentajeCandidatosAsignados => PuestosElectivosTotales > 0
            ? Math.Round((CandidatosAsignadosPuesto * 100.0) / PuestosElectivosTotales, 1)
            : 0;
    }
}
