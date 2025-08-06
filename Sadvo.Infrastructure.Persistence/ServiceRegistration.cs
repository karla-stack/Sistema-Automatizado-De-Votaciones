using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadvo.Core.Domain.Interfaces;
using Sadvo.Core.Domain.Interfaces.AdminInterfaces;
using Sadvo.Core.Domain.Interfaces.DirigenteInterfaces;
using Sadvo.Infrastructure.Persistence.Contexts;
using Sadvo.Infrastructure.Persistence.Repositories;
using Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository;
using Sadvo.Infrastructure.Persistence.Repositories.AdministradorRepository.Sadvo.Infrastructure.Persistence.Repositories.AdminRepositories;
using Sadvo.Infrastructure.Persistence.Repositories.AdminRepositories;
using Sadvo.Infrastructure.Persistence.Repositories.DirigenteRepository;
using Sadvo.Infrastructure.Persistence.Repositories.ElectorRepositories;
using SADVO.Core.Domain.Interfaces.AdminInterfaces;

namespace Sadvo.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {

        public static void AddPersistenceServicesIoc(this IServiceCollection services, IConfiguration configuration)
        {
            #region Contexts
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, m => m.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)), ServiceLifetime.Transient);

            #endregion
            #region Repositories IOC
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IPuestoElectivoRepository, PuestoElectivoRepository>();
            services.AddTransient<ICiudadanoRepository, CiudadanoRepository>();
            services.AddTransient<IPartidoPoliticoRepository, PartidoPoliticoRepository>();
            services.AddTransient<IEleccionesRepository, EleccionRepository>();
            services.AddTransient<IUsuariosRepository, UsuarioRepository>();
            services.AddTransient<IAsignacionDirigenteRepository, AsignacionDirigenteRepository>();
            services.AddTransient<IEleccionesRepository, EleccionRepository>();
            services.AddTransient<IAlianzaPoliticaRepository, AlianzaPoliticaRepository>();
            services.AddTransient<ICandidatoRepository, CandidatoRepository>();
            services.AddTransient<IAsignacionCandidatoRepository, AsignarCandidatoRepository>();
            services.AddTransient<IDirigenteRepository, DirigenteRepository>();
            services.AddTransient<IAdministradorHomeRepository, AdministradorHomeRepository>();
            services.AddTransient<IVotoRepository, VotoRepository>();
            #endregion
        }
    }
}
