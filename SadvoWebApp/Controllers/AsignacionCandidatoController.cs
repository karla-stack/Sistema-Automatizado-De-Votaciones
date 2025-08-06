using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.ViewModels.AsignacionCandidato;

namespace SadvoWebApp.Controllers
{
    public class AsignacionCandidatoController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly IAsignarCandidatoService _asignarCandidatoService;

        public AsignacionCandidatoController(IValidateUserSession session, IAsignarCandidatoService asignacionCandidatoService)
        {
            _session = session;
            _asignarCandidatoService = asignacionCandidatoService;
        }

        // Vista principal de asignaciones
        public async Task<IActionResult> AsignacionCandidatoView()
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
                    ViewBag.SinPartido = true;
                    ViewBag.MensajeSinPartido = "El dirigente no tiene un partido político asignado. Contacte al administrador.";

                    var modelVacio = new AsignacionCandidatoList
                    {
                        Lista = new List<AsignacionCandidatoViewModel>()
                    };
                    return View(modelVacio);
                }

                var lista = await _asignarCandidatoService.GetListAsync();

                var model = new AsignacionCandidatoList
                {
                    Lista = lista.Select(item => new AsignacionCandidatoViewModel
                    {
                        Id = item.Id,
                        Nombre = item.Nombre,
                        Apellido = item.Apellido,
                        PuestoAsignado = item.PuestoElectivo,
                        CandidatoId = item.CandidatoId,
                        PuestoElectivoId = item.PuestoElectivoId 
                    }).ToList()
                };

                // Verificar elección activa
                var validacion = await _asignarCandidatoService.ValidacionEleccionActiva();
                ViewBag.EleccionActiva = !validacion.Exito;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar las asignaciones";
                var modelVacio = new AsignacionCandidatoList
                {
                    Lista = new List<AsignacionCandidatoViewModel>()
                };
                return View(modelVacio);
            }
        }

        // GET: Formulario para agregar nueva asignación
        public async Task<IActionResult> AddAsignacionCandidato()
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                // Validar elección activa
                var validacionEleccion = await _asignarCandidatoService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    TempData["ErrorMessage"] = validacionEleccion.Mensaje;
                    return RedirectToAction("AsignacionCandidatoView");
                }

                var candidatos = await _asignarCandidatoService.ObtenerCandidato();
                var puestos = await _asignarCandidatoService.ObtenerPuestoElectivo();

                var model = new AsignacionCandidatoForm
                {
                    Candidatos = candidatos,
                    Puestos = puestos 
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el formulario";
                return RedirectToAction("AsignacionCandidatoView");
            }
        }

        // POST: Crear asignación
        [HttpPost]
        public async Task<IActionResult> AddAsignacionCandidato(AsignacionCandidatoForm model)
        {
            try
            {
                // Validar elección activa
                var validacionEleccion = await _asignarCandidatoService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    model.Mensaje = validacionEleccion.Mensaje;
                    model.Candidatos = await _asignarCandidatoService.ObtenerCandidato();
                    model.Puestos = await _asignarCandidatoService.ObtenerPuestoElectivo();
                    return View(model);
                }

                if (ModelState.IsValid && model.IdCandidato > 0 && model.IdPuestoElectivo > 0)
                {
                    var resultado = await _asignarCandidatoService.AddAsync(
                        model.IdPuestoElectivo,
                        model.IdCandidato
                    );

                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Asignación creada exitosamente";
                        return RedirectToAction("AsignacionCandidatoView");
                    }
                    else
                    {
                        model.Mensaje = "No se pudo crear la asignación. Verifique las reglas de alianzas.";
                    }
                }
                else
                {
                    model.Mensaje = "Por favor seleccione un candidato y un puesto válidos.";
                }

                // Recargar listas si hay error
                model.Candidatos = await _asignarCandidatoService.ObtenerCandidato();
                model.Puestos = await _asignarCandidatoService.ObtenerPuestoElectivo();
                return View(model);
            }
            catch (Exception ex)
            {
                model.Mensaje = $"Error: {ex.Message}";
                model.Candidatos = await _asignarCandidatoService.ObtenerCandidato();
                model.Puestos = await _asignarCandidatoService.ObtenerPuestoElectivo();
                return View(model);
            }
        }

        // GET: Confirmar eliminación
        public async Task<ActionResult> DeleteAsignacionCandidato(int Id)
        {
            try
            {
                if (!_session.HasUserSession())
                {
                    return RedirectToAction("Login", "Login");
                }

                var dto = await _asignarCandidatoService.GetById(Id);
                if (dto == null)
                {
                    TempData["ErrorMessage"] = "Asignación no encontrada";
                    return RedirectToAction("AsignacionCandidatoView");
                }

                var valorEliminado = new DeleteAsignacionCandidatoViewModel
                {
                    Id = dto.Id,
                    PuestoElectivoId = dto.PuestoElectivoId,
                    CandidatoId = dto.CandidatoId,
                    NombreCandidato = $"{dto.Nombre} {dto.Apellido}",
                    NombrePuestoElectivo = dto.PuestoAsignado
                };

                return View("DeleteAsignacionCandidato", valorEliminado);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la confirmación";
                return RedirectToAction("AsignacionCandidatoView");
            }
        }

        // POST: Eliminar asignación
        [HttpPost]
        public async Task<ActionResult> DeleteAsignacionCandidato(DeleteAsignacionCandidatoViewModel model)
        {
            try
            {
                // Validar elección activa
                var validacionEleccion = await _asignarCandidatoService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    TempData["ErrorMessage"] = validacionEleccion.Mensaje;
                    return RedirectToAction("AsignacionCandidatoView");
                }

                if (ModelState.IsValid)
                {
                    var resultado = await _asignarCandidatoService.DeleteAsync(model.Id);

                    if (resultado)
                    {
                        TempData["SuccessMessage"] = "Asignación eliminada exitosamente";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al eliminar la asignación";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Datos inválidos para eliminar";
                }

                return RedirectToAction("AsignacionCandidatoView");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("AsignacionCandidatoView");
            }
        }
    }
}