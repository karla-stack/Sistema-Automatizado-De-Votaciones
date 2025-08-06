using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class AlianzaActivaDto
    {
        public int Id { get; set; }
        public string? NombrePartidoAliado { get; set; }
        public string? SiglasPartidoAliado { get; set; }
        public DateOnly FechaAceptacion { get; set; }
    }
}
