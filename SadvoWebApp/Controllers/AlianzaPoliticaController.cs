using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.ViewModels.AlianzaPolitica;

namespace SADVOWebApplication.Controllers
{
    public class AlianzaPoliticaController : Controller
    {
        private readonly IAlianzaPoliticaService _alianzaService;
        private readonly IValidateUserSession _session;

        public AlianzaPoliticaController(IAlianzaPoliticaService alianzaService, IValidateUserSession session)
        {
            _alianzaService = alianzaService;
            _session = session;
        }

        public async Task<IActionResult> AlianzaPoliticaView()
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var session = _session.GetUserSession();
                if (session?.PartidoPoliticoId == null)
                {
                    var modelSinPartido = new AlianzaPoliticaIndexViewModel
                    {
                        SinPartido = true
                    };
                    return View(modelSinPartido);
                }

                // Obtener todos los datos de los services
                var solicitudesPendientes = await _alianzaService.GetSolicitudesPendientes();
                var solicitudesRealizadas = await _alianzaService.GetSolicitudesRealizadas();
                var alianzasActivas = await _alianzaService.GetAlianzasActivas();
                var validacion = await _alianzaService.ValidacionEleccionActiva();

                // Mapear a ViewModels
                var model = new AlianzaPoliticaIndexViewModel
                {
                    SolicitudesPendientes = solicitudesPendientes.Select(s => new SolicitudPendienteViewModel
                    {
                        Id = s.Id,
                        NombrePartidoSolicitante = s.NombrePartidoSolicitante,
                        SiglasPartidoSolicitante = s.SiglasPartidoSolicitante,
                        FechaSolicitud = s.FechaSolicitud
                    }).ToList(),

                    SolicitudesRealizadas = solicitudesRealizadas.Select(s => new SolicitudRealizadaViewModel
                    {
                        Id = s.Id,
                        NombrePartidoReceptor = s.NombrePartidoReceptor,
                        SiglasPartidoReceptor = s.SiglasPartidoReceptor,
                        FechaSolicitud = s.FechaSolicitud,
                        Estado = s.Estado
                    }).ToList(),

                    AlianzasActivas = alianzasActivas.Select(a => new AlianzaActivaViewModel
                    {
                        Id = a.Id,
                        NombrePartidoAliado = a.NombrePartidoAliado,
                        SiglasPartidoAliado = a.SiglasPartidoAliado,
                        FechaAceptacion = a.FechaAceptacion
                    }).ToList(),

                    EleccionActiva = !validacion.Exito
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos de alianzas";
                return View(new AlianzaPoliticaIndexViewModel());
            }
        }

        // GET: Formulario para crear solicitud
        public async Task<IActionResult> CrearSolicitud()
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                // Validar elección activa
                var validacion = await _alianzaService.ValidacionEleccionActiva();
                if (!validacion.Exito)
                {
                    TempData["ErrorMessage"] = validacion.Mensaje;
                    return RedirectToAction("AlianzaPoliticaView");
                }

                // Obtener partidos disponibles del service
                var partidosDisponibles = await _alianzaService.GetPartidosDisponiblesParaSolicitud();

                var model = new CrearSolicitudViewModel
                {
                    PartidosDisponibles = partidosDisponibles
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario";
                return RedirectToAction("AlianzaPoliticaView");
            }
        }

        // POST: Crear solicitud
        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(CrearSolicitudViewModel model)
        {
            try
            {
                // Validar elección activa
                var validacion = await _alianzaService.ValidacionEleccionActiva();
                if (!validacion.Exito)
                {
                    model.Mensaje = validacion.Mensaje;
                    model.PartidosDisponibles = await CargarPartidosDisponibles();
                    return View(model);
                }

                if (ModelState.IsValid && model.PartidoReceptorId > 0)
                {
                    var resultado = await _alianzaService.CrearSolicitudAlianza(model.PartidoReceptorId);

                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Solicitud de alianza creada exitosamente";
                        return RedirectToAction("AlianzaPoliticaView");
                    }
                    else
                    {
                        model.Mensaje = "No se pudo crear la solicitud. Puede que ya exista una solicitud pendiente con este partido.";
                    }
                }
                else
                {
                    model.Mensaje = "Por favor seleccione un partido válido.";
                }

                // Recargar partidos si hay error
                model.PartidosDisponibles = await CargarPartidosDisponibles();
                return View(model);
            }
            catch (Exception ex)
            {
                model.Mensaje = $"Error: {ex.Message}";
                model.PartidosDisponibles = await CargarPartidosDisponibles();
                return View(model);
            }
        }

        // POST: Aceptar solicitud
        [HttpPost]
        public async Task<IActionResult> AceptarSolicitud(int id)
        {
            try
            {
                var validacion = await _alianzaService.ValidacionEleccionActiva();
                if (!validacion.Exito)
                {
                    TempData["ErrorMessage"] = validacion.Mensaje;
                    return RedirectToAction("AlianzaPoliticaView");
                }

                var resultado = await _alianzaService.AceptarSolicitud(id);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "Solicitud aceptada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al aceptar la solicitud";
                }

                return RedirectToAction("AlianzaPoliticaView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("AlianzaPoliticaView");
            }
        }

        // POST: Rechazar solicitud
        [HttpPost]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            try
            {
                var validacion = await _alianzaService.ValidacionEleccionActiva();
                if (!validacion.Exito)
                {
                    TempData["ErrorMessage"] = validacion.Mensaje;
                    return RedirectToAction("AlianzaPoliticaView");
                }

                var resultado = await _alianzaService.RechazarSolicitud(id);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "Solicitud rechazada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al rechazar la solicitud";
                }

                return RedirectToAction("AlianzaPoliticaView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("AlianzaPoliticaView");
            }
        }

        // POST: Eliminar solicitud realizada
        [HttpPost]
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            try
            {
                var validacion = await _alianzaService.ValidacionEleccionActiva();
                if (!validacion.Exito)
                {
                    TempData["ErrorMessage"] = validacion.Mensaje;
                    return RedirectToAction("AlianzaPoliticaView");
                }

                var resultado = await _alianzaService.EliminarSolicitudRealizada(id);
                if (resultado)
                {
                    TempData["SuccessMessage"] = "Solicitud eliminada exitosamente";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la solicitud";
                }

                return RedirectToAction("AlianzaPoliticaView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("AlianzaPoliticaView");
            }
        }

        // Método auxiliar limpio
        private async Task<List<PartidoDisponibleDto>> CargarPartidosDisponibles()
        {
            try
            {
                return await _alianzaService.GetPartidosDisponiblesParaSolicitud();
            }
            catch
            {
                return new List<PartidoDisponibleDto>();
            }
        }
    }
}