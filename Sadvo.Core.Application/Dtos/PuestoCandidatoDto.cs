
namespace Sadvo.Core.Application.Dtos
{
    public class PuestoCandidatoDto
    {

        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}"; // Para mostrar en la tabla
        public string? PuestoElectivo { get; set; }

        public int CandidatoId { get; set; }    

        public int PuestoElectivoId { get; set; }       
    }
}
