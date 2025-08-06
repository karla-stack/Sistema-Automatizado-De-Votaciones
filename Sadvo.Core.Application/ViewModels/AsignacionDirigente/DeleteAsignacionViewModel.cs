

namespace Sadvo.Core.Application.ViewModels.AsignacionDirigente
{
    public class DeleteAsignacionViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int PartidoPoliticoId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? SiglasPartido { get; set; }  // para mostrar en vista
        public string? Mensaje { get; set; }

    }
}
