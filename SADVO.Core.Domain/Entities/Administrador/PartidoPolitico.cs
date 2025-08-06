
using Sadvo.Core.Domain.Entities.Dirigente;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class PartidoPolitico
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public required string Siglas { get; set; }
        public string? Logo { get; set; }
        public required Actividad Estado { get; set; }
        public ICollection<AsignacionDirigente>? AsignacionesDirigente { get; set; }
        public ICollection<Candidato>? Candidatos { get; set; }
        public ICollection<AsignacionCandidatoPuesto>? AsignacionesCandidatos { get; set; }
        // Alianzas como Partido B
        public ICollection<AlianzaPolitica>? Alianzas { get; set; } 
        // Solicitudes como Solicitante
        public ICollection<SolicitudAlianza>? Solicitudes { get; set; } 

    }
}
