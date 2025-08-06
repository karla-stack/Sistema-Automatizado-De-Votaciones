
using Sadvo.Core.Domain.Entities.Administrador;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Dirigente
{
    public class AlianzaPolitica
    {
        public int Id { get; set; }

        public int PartidoAId { get; set; }
        public PartidoPolitico PartidoA { get; set; } = null!;

        public int PartidoBId { get; set; }
        public PartidoPolitico PartidoB { get; set; } = null!;

        public DateOnly FechaAlianza { get; set; }

        public Actividad Estado { get; set; }
    }
}
