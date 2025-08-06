using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IPartidoPoliticoService
    {
        Task<bool?> AddAsync(PartidoPoliticoDto partido);
        Task<List<PartidoPolitico>> GetAllListAsync();
        Task<bool?> UpdateAsync(int id, PartidoPoliticoDto partido);
        Task<bool> CambiarEstadoAsync(int id, PartidoPoliticoDto partido);
        Task<PartidoPoliticoDto?> GetById(int id);

        Task<ResultadoOperacion> ValidacionEleccionActiva();

    }
}
