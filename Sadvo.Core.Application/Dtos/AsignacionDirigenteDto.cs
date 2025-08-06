

namespace Sadvo.Core.Application.Dtos
{
    public class AsignacionDirigenteDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PartidoPoliticoId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? SiglasPartido { get; set; }
    }
}
