using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels.Administrador;

namespace SADVOWebApplication.Controllers
{
    public class MenuController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly IAdministradorHomeService _administradorHomeService;

        public MenuController(
            IValidateUserSession session,
            IAdministradorHomeService administradorHomeService)
        {
            _session = session;
            _administradorHomeService = administradorHomeService;
        }

        public async Task<IActionResult> Menu()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            try
            {
                var userSession = _session.GetUserSession();
                if (userSession == null)
                {
                    return RedirectToRoute(new { controller = "Login", action = "Login" });
                }

                // Obtener datos del dashboard
                var dashboardData = await _administradorHomeService.GetDashboardDataAsync();

                var viewModel = new AdministradorMenuViewModel
                {
                    AñosDisponibles = dashboardData.AñosDisponibles,
                    AñoSeleccionado = dashboardData.AñoMasReciente,
                    ResumenElectoral = new List<ResumenElectoralViewModel>(),
                    UsuarioNombre = $"{userSession.Nombre} {userSession.Apellido}",
                    MostrarResumen = false,
                    TieneError = false
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var userSession = _session.GetUserSession();
                var errorViewModel = new AdministradorMenuViewModel
                {
                    AñosDisponibles = new List<int> { DateTime.Now.Year },
                    AñoSeleccionado = DateTime.Now.Year,
                    ResumenElectoral = new List<ResumenElectoralViewModel>(),
                    UsuarioNombre = userSession != null ? $"{userSession.Nombre} {userSession.Apellido}" : "Administrador",
                    MostrarResumen = false,
                    TieneError = true,
                    MensajeError = "Error al cargar datos del sistema"
                };

                return View(errorViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerResumenElectoral(int añoSeleccionado)
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            try
            {
                var userSession = _session.GetUserSession();
                if (userSession == null)
                {
                    return RedirectToRoute(new { controller = "Login", action = "Login" });
                }

                // Obtener años disponibles
                var años = await _administradorHomeService.GetAñosConEleccionesAsync();

                // Obtener resumen electoral del año seleccionado
                var resumenElectoral = await _administradorHomeService.GetResumenElectoralPorAñoAsync(añoSeleccionado);

                var viewModel = new AdministradorMenuViewModel
                {
                    AñosDisponibles = años,
                    AñoSeleccionado = añoSeleccionado,
                    ResumenElectoral = resumenElectoral.Select(r => new ResumenElectoralViewModel
                    {
                        EleccionId = r.EleccionId,
                        NombreEleccion = r.NombreEleccion,
                        FechaEleccion = r.FechaEleccion,
                        CantidadPartidos = r.CantidadPartidos,
                        CantidadCandidatos = r.CantidadCandidatos,
                        TotalVotos = r.TotalVotos,
                        EstadoEleccion = r.EstadoEleccion
                    }).ToList(),
                    UsuarioNombre = $"{userSession.Nombre} {userSession.Apellido}",
                    MostrarResumen = true,
                    TieneError = false
                };

                return View("Menu", viewModel);
            }
            catch (Exception ex)
            {
                var userSession = _session.GetUserSession();

                // En caso de error, cargar datos básicos
                var años = new List<int> { DateTime.Now.Year };
                try
                {
                    años = await _administradorHomeService.GetAñosConEleccionesAsync();
                }
                catch { }

                var errorViewModel = new AdministradorMenuViewModel
                {
                    AñosDisponibles = años,
                    AñoSeleccionado = añoSeleccionado,
                    ResumenElectoral = new List<ResumenElectoralViewModel>(),
                    UsuarioNombre = userSession != null ? $"{userSession.Nombre} {userSession.Apellido}" : "Administrador",
                    MostrarResumen = true,
                    TieneError = true,
                    MensajeError = $"Error al obtener resumen electoral del año {añoSeleccionado}"
                };

                return View("Menu", errorViewModel);
            }
        }

        // Métodos de navegación del menú
        public IActionResult PuestoElectivo()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "PuestoElectivo");
        }

        public IActionResult Ciudadanos()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "Ciudadano");
        }

        public IActionResult PartidosPoliticos()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "PartidoPolitico");
        }

        public IActionResult Usuarios()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "Usuario");
        }

        public IActionResult AsignacionDirigentes()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("AsignacionDirigentesView", "AsignacionDirigente");
        }

        public IActionResult Elecciones()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("EleccionView", "Eleccion");
        }
    }
}