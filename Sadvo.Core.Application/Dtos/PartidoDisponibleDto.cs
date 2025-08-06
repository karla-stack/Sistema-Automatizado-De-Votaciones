using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadvo.Core.Application.Dtos
{
    public class PartidoDisponibleDto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Siglas { get; set; }
        public string NombreCompleto => $"{Nombre} ({Siglas})";
    }
}
