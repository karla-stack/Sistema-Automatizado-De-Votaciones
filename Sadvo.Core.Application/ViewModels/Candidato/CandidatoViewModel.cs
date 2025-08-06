
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Sadvo.Core.Domain.Enums;

namespace Sadvo.Core.Application.ViewModels.Candidato
{
    public class CandidatoViewModel
    {
        public int Id { get; set; }
        public string ?Nombre { get; set; }
        public string? Apellido { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? Foto { get; set; }

        public string? FotoPath { get; set; }
        public Actividad Estado { get; set; }
        public int PartidoPoliticoId { get; set; }
        public string? PuestoAsociado { get; set; }

        public string? Mensaje{ get; set; }
    }
}
