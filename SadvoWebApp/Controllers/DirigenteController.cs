using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.ViewModels.Dirigente;
using Sadvo.Core.Application.ViewModels.PartidoPolitico;

namespace SADVOWebApplication.Controllers
{
    public class DirigenteController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly IDirigenteService _dirigenteService;

        public DirigenteController(IValidateUserSession session, IDirigenteService dirigenteService)
        {
            _session = session;
            _dirigenteService = dirigenteService;
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

                var dashboardData = await _dirigenteService.GetDashboardAsync(userSession.Id);

                var viewModel = new DirigenteMenuViewModel
                {
                    PartidoInfo = new PartidoPoliticoHomeViewModel
                    {
                        Id = dashboardData.PartidoInfo.Id,
                        Nombre = dashboardData.PartidoInfo.Nombre,
                        Siglas = dashboardData.PartidoInfo.Siglas,
                        LogoUrl = dashboardData.PartidoInfo.LogoUrl,
                        Descripcion = dashboardData.PartidoInfo.Descripcion,
                        EstaActivo = dashboardData.PartidoInfo.EstaActivo
                    },
                    Indicadores = new IndicadoresDirigenteViewModel
                    {
                        CandidatosActivos = dashboardData.Indicadores.CandidatosActivos,
                        CandidatosInactivos = dashboardData.Indicadores.CandidatosInactivos,
                        AlianzasPoliticas = dashboardData.Indicadores.AlianzasPoliticas,
                        SolicitudesAlianzasPendientes = dashboardData.Indicadores.SolicitudesAlianzasPendientes,
                        CandidatosAsignadosPuesto = dashboardData.Indicadores.CandidatosAsignadosPuesto,
                        PuestosElectivosTotales = dashboardData.Indicadores.PuestosElectivosTotales,
                        PorcentajeCandidatosAsignados = dashboardData.Indicadores.PorcentajeCandidatosAsignados
                    },
                    TieneEleccionActiva = dashboardData.TieneEleccionActiva,
                    EleccionActivaNombre = dashboardData.EleccionActivaNombre,
                    UsuarioNombre = $"{userSession.Nombre} {userSession.Apellido}",
                    TieneError = false
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                var userSession = _session.GetUserSession();
                var errorViewModel = new DirigenteMenuViewModel
                {
                    PartidoInfo = new PartidoPoliticoHomeViewModel { Nombre = "Error", Siglas = "N/A" },
                    Indicadores = new IndicadoresDirigenteViewModel(),
                    UsuarioNombre = userSession != null ? $"{userSession.Nombre} {userSession.Apellido}" : "Usuario",
                    TieneError = true,
                    MensajeError = "Error al cargar datos del partido"
                };

                return View(errorViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetIndicadores()
        {
            if (!_session.HasUserSession())
            {
                return Json(new { success = false, message = "Sesión expirada" });
            }

            try
            {
                var userSession = _session.GetUserSession();
                if (userSession == null)
                {
                    return Json(new { success = false, message = "Usuario no encontrado" });
                }

                var indicadores = await _dirigenteService.GetIndicadoresAsync(userSession.Id);

                return Json(new
                {
                    success = true,
                    data = indicadores
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener indicadores" });
            }
        }

        public IActionResult Candidatos()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "Candidato");
        }

        public IActionResult AsignarCandidatos()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "AsignacionCandidato");
        }

        public IActionResult AlianzasPoliticas()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            return RedirectToAction("Menu", "AlianzaPolitica");
        }
    }
}