
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Domain.Entities.Administrador;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface ICiudadanoService
    {
        Task<bool?> AddAsync(CiudadanoDto ciudadano);
        Task<List<Ciudadano>> GetAllListAsync();
        Task<bool?> UpdateAsync(int id, CiudadanoDto ciudadano);

        Task<ResultadoOperacion> ValidacionEleccionActiva();

        Task<bool> CambiarEstadoAsync(int id, CiudadanoDto ciudadano);

        Task<CiudadanoDto?> GetById(int id);

        Task<ResultadoOperacion> ValidacionCedulaExistente(CiudadanoDto ciudadano);
    }
}
