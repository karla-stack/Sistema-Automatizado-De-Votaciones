using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.Services.AdministradorServices;
using Sadvo.Core.Application.Services.DirigenteServices;
using SADVO.Core.Application.Services;

namespace Sadvo.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            #region Services IOC
            // services.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddScoped<ICiudadanoService, CiudadanoService>();
            services.AddScoped<IPuestoElectivoService, PuestoElectivoService>();
            services.AddScoped<IPartidoPoliticoService, PartidoPoliticoService>();
            services.AddScoped<ICiudadanoService, CiudadanoService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAsignacionDirigenteService, AsignacionDirigenteService>();
            services.AddScoped<IEleccionService, EleccionService>();
            services.AddScoped<ICandidatoService, CandidatoService>();
            services.AddScoped<IAsignarCandidatoService, AsignarCandidatoService>();
            services.AddScoped<IAlianzaPoliticaService, AlianzaPoliticaService>();
            services.AddScoped<IDirigenteService, DirigenteService>();
            services.AddScoped<IAdministradorHomeService, AdministradorHomeService>();
            services.AddScoped<IOcrService, TesseractOcrService>();
            #endregion
        }
    }
}
