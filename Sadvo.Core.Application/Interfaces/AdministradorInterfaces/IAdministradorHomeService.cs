

using Sadvo.Core.Application.Dtos;

namespace Sadvo.Core.Application.Interfaces.AdministradorInterfaces
{
    public interface IAdministradorHomeService
    {
        Task<AdministradorHomeDto> GetDashboardDataAsync();
        Task<List<int>> GetAñosConEleccionesAsync();
        Task<List<ResumenElectoralDto>> GetResumenElectoralPorAñoAsync(int año);
    }
}
