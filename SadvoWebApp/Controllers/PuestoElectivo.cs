using System;
using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels;
using Sadvo.Core.Application.ViewModels.PuestoElectivo;


namespace SADVOWebApplication.Controllers
{
    public class PuestoElectivoController : Controller
    {

        private readonly IValidateUserSession _session;

        private readonly IPuestoElectivoService _puestoElectivoService;

        public string Mensaje { get; set; } = string.Empty;

        public PuestoElectivoController(IValidateUserSession session, IPuestoElectivoService puestoElectivoService)
        {
            _session = session;
            _puestoElectivoService = puestoElectivoService;
        }


        //PuestoElectivo//
        public async Task<IActionResult> PuestoElectivoView()
        {

            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            var puestos = await _puestoElectivoService.GetAllListAsync();

            var lista = puestos.Select(p => new PuestoElectivoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Estado = p.Estado
            }).ToList();

            var vm = new PuestoElectivoListViewModel
            {
                Puestos = lista
            };

            return View(vm);
        }


        public ActionResult AddPuestoElectivo()
        {
            var model = new PuestoElectivoViewModel();
            ViewBag.IsEdit = false;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddPuestoElectivo(PuestoElectivoViewModel vm)
        {

            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            ViewBag.IsEdit = false;

            // Validación 1: Elección activa
            var validacionEleccion = await _puestoElectivoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                vm.Mensaje = validacionEleccion.Mensaje;
                return View("AddPuestoElectivo", vm);
            }

            if (!ModelState.IsValid)
            {
                return View("AddPuestoElectivo", vm);
            }

            PuestoElectivoDto dto = new()
            {
                Id = 0,
                Nombre = vm.Nombre,
                Descripcion = vm.Descripcion,
                Estado = Sadvo.Core.Domain.Enums.Actividad.Activo
            };

            var resultado = await _puestoElectivoService.AddAsync(dto);


            return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });

        }


        public async Task<IActionResult> EditPuestoElectivo(int id)
        {
            ViewBag.IsEdit = true;

            var entity = await _puestoElectivoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });
            }

            else
            {
                PuestoElectivoViewModel valorActualizado = new() { Id = entity.Id, Nombre = entity.Nombre, Descripcion = entity.Descripcion, Estado = entity.Estado };

                return View("AddPuestoElectivo", valorActualizado);

            }
        }

        [HttpPost]
        public async Task<ActionResult> EditPuestoElectivo(PuestoElectivoViewModel model, int id)
        {

            ViewBag.IsEdit = true;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _puestoElectivoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });
            }

            else
            {
                // Validación 1: Elección activa
                var validacionEleccion = await _puestoElectivoService.ValidacionEleccionActiva();
                if (!validacionEleccion.Exito)
                {
                    model.Mensaje = validacionEleccion.Mensaje;
                    return View("AddPuestoElectivo", model);
                }

                PuestoElectivoDto dto = new() { Id = model.Id, Nombre = model.Nombre, Descripcion = model.Descripcion, Estado = model.Estado };

                var resultado = await _puestoElectivoService.UpdateAsync(id, dto);

                return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });

            }
        }
        // GET: Mostrar pantalla de confirmación
        [HttpGet]
        public async Task<ActionResult> ConfirmarCambioEstado(int id)
        {
            var entity = await _puestoElectivoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });
            }

            var viewModel = new ConfirmarEstadoViewModel
            {
                UsuarioId = id,
                EstadoActual = entity.Estado,
                NuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo
            };

            return View("ConfirmarCambioEstado", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstadoUsuario(ConfirmarEstadoViewModel model, int id)
        {
            // Ensure the model is properly initialized
            if (model == null)
            {
                model = new ConfirmarEstadoViewModel();
            }

            var entity = await _puestoElectivoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });
            }

            var nuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo;
            model.EstadoActual = entity.Estado;
            model.NuevoEstado = nuevoEstado;
            model.UsuarioId = id;

            // Validación 1: Elección activa
            var validacionEleccion = await _puestoElectivoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                // Return to the same confirmation view with error message
                return View("ConfirmarCambioEstado", model);
            }

            PuestoElectivoDto estadoDto = new()
            {
                Id = id,
                Estado = nuevoEstado
            };

            await _puestoElectivoService.CambiarEstadoAsync(id, estadoDto);

            return RedirectToRoute(new { controller = "PuestoElectivo", action = "PuestoElectivoView" });
        }
    }
}
