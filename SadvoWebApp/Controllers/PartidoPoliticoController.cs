using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.AdministradorInterfaces;
using Sadvo.Core.Application.ViewModels;
using Sadvo.Core.Application.ViewModels.PartidoPolitico;

namespace SADVOWebApplication.Controllers
{
    public class PartidoPoliticoController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly IPartidoPoliticoService _partidoPoliticoService;

        public string Mensaje { get; set; } = string.Empty;

        public PartidoPoliticoController(IValidateUserSession session, IPartidoPoliticoService partidoPoliticoService)
        {
            _session = session;
            _partidoPoliticoService = partidoPoliticoService;

            // Asegurar que existan las carpetas de uploads
            FileHelper.EnsureUploadFoldersExist();
        }

        public async Task<ActionResult> PartidoPoliticoView()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            // Verificar si hay elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            var partidos = await _partidoPoliticoService.GetAllListAsync();

            var lista = partidos.Select(p => new PartidoPoliticoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Siglas = p.Siglas,
                Estado = p.Estado,
                LogoPath = p.Logo // ← Mapear logo de BD a LogoPath para mostrar
            }).ToList();

            var vm = new PartidoPoliticoListViewModel
            {
                Partidos = lista
            };
            return View(vm);
        }

        public async Task<ActionResult> AddPartidoPolitico()
        {
            var model = new PartidoPoliticoViewModel();

            // Verificar si hay elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartidoPolitico(PartidoPoliticoViewModel model)
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            // Validación: Elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("AddPartidoPolitico", model);
            }

            if (!ModelState.IsValid)
            {
                return View("AddPartidoPolitico", model);
            }

            // 🏛️ MANEJAR UPLOAD DE LOGO
            string logoPath = null;
            if (model.Logo != null && model.Logo.Length > 0)
            {
                // Validar archivo
                if (!model.Logo.IsValid(out string errorMessage))
                {
                    ModelState.AddModelError("Logo", errorMessage);
                    return View("AddPartidoPolitico", model);
                }

                // Subir archivo
                logoPath = await model.Logo.SaveToServerAsync("partidos");

                if (logoPath == null)
                {
                    ModelState.AddModelError("Logo", "Error al subir el logo");
                    return View("AddPartidoPolitico", model);
                }
            }

            PartidoPoliticoDto dto = new()
            {
                Id = 0,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Siglas = model.Siglas,
                Estado = model.Estado,
                Logo = logoPath // ← Esta ruta se guarda en la BD
            };

            try
            {
                var resultado = await _partidoPoliticoService.AddAsync(dto);

                if (resultado != null)
                {
                    TempData["Success"] = "Partido político creado exitosamente";
                }
                else
                {
                    // Si falló, eliminar logo subido
                    if (logoPath != null)
                        FileHelper.DeleteFile(logoPath);

                    model.Mensaje = "Error al crear el partido político";
                    return View("AddPartidoPolitico", model);
                }
            }
            catch (Exception ex)
            {
                // Si falló, eliminar logo subido
                if (logoPath != null)
                    FileHelper.DeleteFile(logoPath);

                model.Mensaje = $"Error: {ex.Message}";
                return View("AddPartidoPolitico", model);
            }

            return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
        }

        public async Task<IActionResult> EditPartidoPolitico(int id)
        {
            ViewBag.IsEdit = true;

            var entity = await _partidoPoliticoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
            }

            // Verificar si hay elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            PartidoPoliticoViewModel valorActualizado = new()
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Descripcion = entity.Descripcion,
                Siglas = entity.Siglas,
                Estado = entity.Estado,
                LogoPath = entity.Logo // ← Para mostrar el logo actual
                // Logo se deja null (es para subir nuevo)
            };

            return View("AddPartidoPolitico", valorActualizado);
        }

        [HttpPost]
        public async Task<ActionResult> EditPartidoPolitico(PartidoPoliticoViewModel model, int id)
        {
            ViewBag.IsEdit = true;

            if (!ModelState.IsValid)
            {
                return View("AddPartidoPolitico", model);
            }

            var entity = await _partidoPoliticoService.GetById(id);

            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
            }

            // Validación: Elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("AddPartidoPolitico", model);
            }

            // 🏛️ MANEJAR UPLOAD DE NUEVO LOGO
            string nuevoLogoPath = null;
            if (model.Logo != null && model.Logo.Length > 0)
            {
                // Validar archivo
                if (!model.Logo.IsValid(out string errorMessage))
                {
                    ModelState.AddModelError("Logo", errorMessage);
                    return View("AddPartidoPolitico", model);
                }

                // Subir nuevo logo
                nuevoLogoPath = await model.Logo.SaveToServerAsync("partidos");

                if (nuevoLogoPath == null)
                {
                    ModelState.AddModelError("Logo", "Error al subir el logo");
                    return View("AddPartidoPolitico", model);
                }

                // Eliminar logo anterior si existe
                if (!string.IsNullOrEmpty(entity.Logo))
                {
                    FileHelper.DeleteFile(entity.Logo);
                }
            }

            PartidoPoliticoDto dto = new()
            {
                Id = model.Id,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Siglas = model.Siglas,
                Estado = model.Estado,
                Logo = nuevoLogoPath ?? entity.Logo // Nuevo logo o mantener el actual
            };

            try
            {
                var resultado = await _partidoPoliticoService.UpdateAsync(id, dto);

                if (resultado != null)
                {
                    TempData["Success"] = "Partido político actualizado exitosamente";
                }
                else
                {
                    // Si falló, eliminar nuevo logo subido
                    if (nuevoLogoPath != null)
                        FileHelper.DeleteFile(nuevoLogoPath);

                    model.Mensaje = "Error al actualizar el partido político";
                    return View("AddPartidoPolitico", model);
                }
            }
            catch (Exception ex)
            {
                // Si falló, eliminar nuevo logo subido
                if (nuevoLogoPath != null)
                    FileHelper.DeleteFile(nuevoLogoPath);

                model.Mensaje = $"Error: {ex.Message}";
                return View("AddPartidoPolitico", model);
            }

            return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmarCambioEstado(int id)
        {
            var entity = await _partidoPoliticoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
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
            if (model == null)
            {
                model = new ConfirmarEstadoViewModel();
            }

            var entity = await _partidoPoliticoService.GetById(id);
            if (entity == null)
            {
                return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
            }

            var nuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo ? Sadvo.Core.Domain.Enums.Actividad.Inactivo : Sadvo.Core.Domain.Enums.Actividad.Activo;
            model.EstadoActual = entity.Estado;
            model.NuevoEstado = nuevoEstado;
            model.UsuarioId = id;

            // Validación: Elección activa
            var validacionEleccion = await _partidoPoliticoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("ConfirmarCambioEstado", model);
            }

            PartidoPoliticoDto estadoDto = new()
            {
                Id = id,
                Estado = nuevoEstado
            };

            try
            {
                await _partidoPoliticoService.CambiarEstadoAsync(id, estadoDto);
                TempData["Success"] = $"Partido político {(nuevoEstado == Sadvo.Core.Domain.Enums.Actividad.Activo ? "activado" : "desactivado")} exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToRoute(new { controller = "PartidoPolitico", action = "PartidoPoliticoView" });
        }
    }
}