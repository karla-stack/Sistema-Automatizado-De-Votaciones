using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels.AsignacionDirigente;

namespace SADVOWebApplication.Controllers
{
    public class AsignacionDirigenteController : Controller
    {



        private readonly IValidateUserSession _session;

        private readonly IAsignacionDirigenteService _asignacionDirigenteService;

        public string Mensaje { get; set; } = string.Empty;

        public AsignacionDirigenteController(IValidateUserSession session, IAsignacionDirigenteService asignacionDirigenteService)
        {
            _session = session;
            _asignacionDirigenteService = asignacionDirigenteService;
        }


        //AsignacionDirigente//
        public async Task<ActionResult> AsignacionDirigentesView()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            var resultado = await _asignacionDirigenteService.GetListAsync();

            var listaDirigentePartido = resultado.Select(p => new AsignacionDirigenteViewModel
            {
                Id = p.Id,
                NombreUsuario = p.NombreUsuario,
                SiglasPartido = p.SiglasPartido

            }).ToList();

            var vm = new AsignacionDirigenteListViewModel
            {
                lista = listaDirigentePartido
            };
            return View(vm);
        }




        public async Task<IActionResult> AddAsignacionDirigente()
        {
            var partidos = await _asignacionDirigenteService.ObtenerPartidoPolitico();
            var usuarios = await _asignacionDirigenteService.ObtenerUsuarios();

            var model = new AsignacionDirigenteFormViewModel
            {
                Partidos = partidos,
                Usuarios = usuarios
            };

            ViewBag.Partidos = partidos.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Siglas
            }).ToList();

            ViewBag.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = $"{u.Nombre} {u.Apellido}"
            }).ToList();

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddAsignacionDirigente(AsignacionDirigenteFormViewModel model)
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            if (!ModelState.IsValid)
            {
                return View(model); // en caso de error, vuelve a la vista
            }


            // Validación 1: Elección activa
            var validacionEleccion = await _asignacionDirigenteService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("AddAsignacionDirigente", model);
            }
            await _asignacionDirigenteService.AddAsync(model.IdPartido, model.IdUsuario);

            return RedirectToAction("AsignacionDirigentesView");
        }



        public async Task<ActionResult> DeleteAsignacionDirigente(int Id)
        {
            var dto = await _asignacionDirigenteService.GetById(Id);

            if (dto == null)
            {
                return RedirectToRoute(new { controller = "AsignacionDirigentesView", action = "AsignacionDirigentesView" });
            }

            else
            {
                DeleteAsignacionViewModel valorEliminado = new()
                {
                    Id = dto.Id,
                    UsuarioId = dto.UsuarioId,
                    PartidoPoliticoId = dto.PartidoPoliticoId,
                    NombreUsuario = dto.NombreUsuario,
                    SiglasPartido = dto.SiglasPartido
                };

                return View("DeleteAsignacionDirigente", valorEliminado);


            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteAsignacionDirigente(DeleteAsignacionViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validación 1: Elección activa
            var validacionEleccion = await _asignacionDirigenteService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("DeleteAsignacionDirigente", model);
            }

            var resultado = await _asignacionDirigenteService.DeleteAsync(model.Id);

            return RedirectToRoute(new { controller = "AsignacionDirigente", action = "AsignacionDirigentesView" });
        }



    }
}
