
namespace Sadvo.Core.Domain.Entities.Administrador
{
    public class AsignacionDirigente
    {
        public int Id { get; set; }
        public Usuario? Usuario { get; set; }
        public int UsuarioId { get; set; }
        public PartidoPolitico? PartidoPolitico { get; set; }
        public int PartidoPoliticoId { get; set; }

    }
}
