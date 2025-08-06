using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class AdministradorHomeDto
    {
        public List<int> AñosDisponibles { get; set; } = new();
        public int AñoMasReciente { get; set; }
        public List<ResumenElectoralDto> ResumenElectoral { get; set; } = new();
    }
}
