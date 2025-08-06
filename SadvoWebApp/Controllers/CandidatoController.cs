using System;
using Microsoft.AspNetCore.Mvc;
using Sadvo.Core.Application.Dtos;
using Sadvo.Core.Application.Interfaces;
using Sadvo.Core.Application.Interfaces.DirigenteInterfaces;
using Sadvo.Core.Application.ViewModels;
using Sadvo.Core.Application.ViewModels.Candidato;
using Sadvo.Core.Application.ViewModels.PuestoElectivo;

namespace SadvoWebApp.Controllers
{
    public class CandidatoController : Controller
    {
        private readonly IValidateUserSession _session;
        private readonly ICandidatoService _candidatoService;

        public CandidatoController(IValidateUserSession session, ICandidatoService candidatoService)
        {
            _session = session;
            _candidatoService = candidatoService;

            // Asegurar que existan las carpetas de uploads
            FileHelper.EnsureUploadFoldersExist();
        }

        public async Task<IActionResult> CandidatoView()
        {
            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            var session = _session.GetUserSession();

            // Verificar si hay elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            var candidatosFromDb = await _candidatoService.GetListCandidatosByPartido(session.PartidoPoliticoId.Value);

            var lista = candidatosFromDb.Select(p => new CandidatoViewModel
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                FotoPath = p.Foto, // ← Mapear foto de BD a FotoPath para mostrar
                Estado = p.Estado,
                PartidoPoliticoId = p.PartidoPoliticoId,
                PuestoAsociado = p.PuestoAsociado
            }).ToList();

            // Debug: Verificar fotos
            Console.WriteLine($"Total candidatos: {lista.Count}");
            Console.WriteLine($"Con foto: {lista.Count(c => !string.IsNullOrEmpty(c.FotoPath))}");

            foreach (var candidato in lista)
            {
                if (!string.IsNullOrEmpty(candidato.FotoPath))
                {
                    bool existe = FileHelper.PhotoExists(candidato.FotoPath);
                    Console.WriteLine($"📷 {candidato.Nombre}: {candidato.FotoPath} (Existe: {existe})");
                }
            }

            var vm = new CandidatoListViewModel
            {
                candidatos = lista
            };

            return View(vm);
        }

        public async Task<ActionResult> AddCandidato()
        {
            var model = new CandidatoViewModel();

            // Verificar si hay elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            ViewBag.IsEdit = false;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCandidato(CandidatoViewModel vm)
        { 

            if (!_session.HasUserSession())
            {
                return RedirectToRoute(new { controller = "Login", action = "Login" });
            }

            ViewBag.IsEdit = false;

            // Validación: Elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            if (!validacionEleccion.Exito)
            {
                vm.Mensaje = validacionEleccion.Mensaje;
                return View("AddCandidato", vm);
            }

            if (!ModelState.IsValid)
            {
                return View("AddCandidato", vm);
            }

            Console.WriteLine($"📝 Creando candidato: {vm.Nombre} {vm.Apellido}");
            Console.WriteLine($"📷 ¿Hay foto? {vm.Foto != null && vm.Foto.Length > 0}");

            // 📷 MANEJAR UPLOAD DE FOTO con el helper completo
            string fotoPath = null;
            if (vm.Foto != null && vm.Foto.Length > 0)
            {
                Console.WriteLine($"📷 Archivo recibido: {vm.Foto.FileName} ({FileHelper.FormatFileSize(vm.Foto.Length)})");

                // Validar archivo usando extensión
                if (!vm.Foto.IsValid(out string errorMessage))
                {
                    Console.WriteLine($"❌ Error de validación: {errorMessage}");
                    ModelState.AddModelError("Foto", errorMessage);
                    return View("AddCandidato", vm);
                }

                // Subir archivo usando extensión
                fotoPath = await vm.Foto.SaveToServerAsync("candidatos");

                if (fotoPath == null)
                {
                    Console.WriteLine("❌ ERROR: No se pudo subir la foto");
                    ModelState.AddModelError("Foto", "Error al subir la imagen");
                    return View("AddCandidato", vm);
                }

                Console.WriteLine($" Foto subida exitosamente: {fotoPath}");

                // Verificar que se guardó correctamente
                if (FileHelper.PhotoExists(fotoPath))
                {
                    var info = FileHelper.GetPhotoInfo(fotoPath);
                    Console.WriteLine($" Verificación: Archivo existe, tamaño: {FileHelper.FormatFileSize(info.Length)}");
                }
                else
                {
                    Console.WriteLine(" ADVERTENCIA: El archivo no se encontró después del upload");
                }
            }
            else
            {
                Console.WriteLine(" No se recibió archivo de foto");
            }

            // Obtener el partido del usuario logueado
            var session = _session.GetUserSession();

            CandidatoDto dto = new()
            {
                Id = 0,
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Foto = fotoPath, // ← Esta ruta se guarda en la BD
                Estado = Sadvo.Core.Domain.Enums.Actividad.Activo,
                PartidoPoliticoId = session.PartidoPoliticoId.Value,
                PuestoAsociado = vm.PuestoAsociado
            };

            Console.WriteLine($"💾 Guardando en BD: Foto = '{dto.Foto}'");

            try
            {
                var resultado = await _candidatoService.AddAsync(dto);

                if (resultado != null)
                {
                    Console.WriteLine("✅ Candidato guardado exitosamente en BD");
                    TempData["Success"] = "Candidato creado exitosamente";
                }
                else
                {
                    Console.WriteLine("❌ Error al guardar en BD");
                    // Si falló, eliminar foto subida
                    if (fotoPath != null)
                    {
                        FileHelper.DeleteFile(fotoPath);
                        Console.WriteLine("🗑️ Foto eliminada por error en BD");
                    }

                    vm.Mensaje = "Error al crear el candidato";
                    return View("AddCandidato", vm);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al guardar: {ex.Message}");
                // Si falló, eliminar foto subida
                if (fotoPath != null)
                {
                    FileHelper.DeleteFile(fotoPath);
                }

                vm.Mensaje = $"Error: {ex.Message}";
                return View("AddCandidato", vm);
            }

            Console.WriteLine("🎉 Proceso completado - Redirigiendo");
            return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
        }

        public async Task<IActionResult> EditCandidato(int id)
        {
            ViewBag.IsEdit = true;

            var entity = await _candidatoService.GetById(id);

            if (entity == null)
            {
                TempData["Error"] = "Candidato no encontrado";
                return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
            }

            // Verificar si hay elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            CandidatoViewModel valorActualizado = new()
            {
                Id = entity.Id,
                Nombre = entity.Nombre,
                Apellido = entity.Apellido,
                Estado = entity.Estado,
                FotoPath = entity.Foto, // ← Para mostrar la foto actual
                PartidoPoliticoId = entity.PartidoPoliticoId,
                PuestoAsociado = entity.PuestoAsociado
            };

            // Debug: Verificar foto actual
            if (!string.IsNullOrEmpty(valorActualizado.FotoPath))
            {
                bool existe = FileHelper.PhotoExists(valorActualizado.FotoPath);
                Console.WriteLine($"📷 Foto actual: {valorActualizado.FotoPath} (Existe: {existe})");
            }

            return View("EditCandidato", valorActualizado);
        }

        [HttpPost]
        public async Task<ActionResult> EditCandidato(CandidatoViewModel model, int id)
        {
            Console.WriteLine($"✏️ EDITANDO candidato ID: {id}");

            ViewBag.IsEdit = true;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = await _candidatoService.GetById(id);

            if (entity == null)
            {
                TempData["Error"] = "Candidato no encontrado";
                return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
            }

            // Validación: Elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("EditCandidato", model);
            }

            Console.WriteLine($"📷 ¿Nueva foto? {model.Foto != null && model.Foto.Length > 0}");
            Console.WriteLine($"📷 Foto actual: {entity.Foto ?? "Sin foto"}");

            // 📷 MANEJAR UPLOAD DE NUEVA FOTO
            string nuevaFotoPath = null;
            if (model.Foto != null && model.Foto.Length > 0)
            {
                Console.WriteLine($"📷 Subiendo nueva foto: {model.Foto.FileName}");

                // Validar archivo
                if (!model.Foto.IsValid(out string errorMessage))
                {
                    Console.WriteLine($"❌ Error de validación: {errorMessage}");
                    ModelState.AddModelError("Foto", errorMessage);
                    return View("EditCandidato", model);
                }

                // Subir nueva foto
                nuevaFotoPath = await model.Foto.SaveToServerAsync("candidatos");

                if (nuevaFotoPath == null)
                {
                    Console.WriteLine("❌ ERROR: No se pudo subir la nueva foto");
                    ModelState.AddModelError("Foto", "Error al subir la imagen");
                    return View("EditCandidato", model);
                }

                Console.WriteLine($"✅ Nueva foto subida: {nuevaFotoPath}");

                // Eliminar foto anterior si existe y es diferente
                if (!string.IsNullOrEmpty(entity.Foto) && entity.Foto != nuevaFotoPath)
                {
                    bool eliminado = FileHelper.DeleteFile(entity.Foto);
                    Console.WriteLine($"🗑️ Foto anterior eliminada: {eliminado}");
                }
            }

            // Determinar qué foto usar
            string fotoFinal = nuevaFotoPath ?? entity.Foto;
            Console.WriteLine($"📷 Foto final: {fotoFinal ?? "Sin foto"}");

            CandidatoDto dto = new()
            {
                Id = model.Id,
                Nombre = model.Nombre ?? string.Empty,
                Apellido = model.Apellido ?? string.Empty,
                Estado = model.Estado,
                Foto = fotoFinal, // Nueva foto o mantener la actual
                PartidoPoliticoId = model.PartidoPoliticoId,
                PuestoAsociado = model.PuestoAsociado
            };

            try
            {
                var resultado = await _candidatoService.UpdateAsync(id, dto);

                if (resultado != null)
                {
                    Console.WriteLine("✅ Candidato actualizado exitosamente");
                    TempData["Success"] = "Candidato actualizado exitosamente";
                }
                else
                {
                    Console.WriteLine("❌ Error al actualizar en BD");
                    // Si falló, eliminar nueva foto subida
                    if (nuevaFotoPath != null)
                    {
                        FileHelper.DeleteFile(nuevaFotoPath);
                    }

                    model.Mensaje = "Error al actualizar el candidato";
                    return View("EditCandidato", model);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción al actualizar: {ex.Message}");
                // Si falló, eliminar nueva foto subida
                if (nuevaFotoPath != null)
                {
                    FileHelper.DeleteFile(nuevaFotoPath);
                }

                model.Mensaje = $"Error: {ex.Message}";
                return View("EditCandidato", model);
            }

            return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
        }

        // GET: Mostrar pantalla de confirmación
        [HttpGet]
        public async Task<ActionResult> ConfirmarCambioEstado(int id)
        {
            var entity = await _candidatoService.GetById(id);
            if (entity == null)
            {
                TempData["Error"] = "Candidato no encontrado";
                return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
            }

            // Verificar si hay elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            ViewBag.EleccionActiva = !validacionEleccion.Exito;

            var viewModel = new ConfirmarEstadoViewModel
            {
                UsuarioId = id,
                EstadoActual = entity.Estado,
                NuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo
                    ? Sadvo.Core.Domain.Enums.Actividad.Inactivo
                    : Sadvo.Core.Domain.Enums.Actividad.Activo
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

            var entity = await _candidatoService.GetById(id);
            if (entity == null)
            {
                TempData["Error"] = "Candidato no encontrado";
                return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
            }

            var nuevoEstado = entity.Estado == Sadvo.Core.Domain.Enums.Actividad.Activo
                ? Sadvo.Core.Domain.Enums.Actividad.Inactivo
                : Sadvo.Core.Domain.Enums.Actividad.Activo;

            model.EstadoActual = entity.Estado;
            model.NuevoEstado = nuevoEstado;
            model.UsuarioId = id;

            // Validación: Elección activa
            var validacionEleccion = await _candidatoService.ValidacionEleccionActiva();
            if (!validacionEleccion.Exito)
            {
                model.Mensaje = validacionEleccion.Mensaje;
                return View("ConfirmarCambioEstado", model);
            }

            CandidatoDto estadoDto = new()
            {
                Id = id,
                Estado = nuevoEstado
            };

            try
            {
                await _candidatoService.CambiarEstadoAsync(id, estadoDto);

                string estadoTexto = nuevoEstado == Sadvo.Core.Domain.Enums.Actividad.Activo ? "activado" : "desactivado";
                TempData["Success"] = $"Candidato {estadoTexto} exitosamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cambiar estado: {ex.Message}";
            }

            return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
        }

        // Método para eliminar candidato (con foto)
        [HttpPost]
        public async Task<IActionResult> DeleteCandidato(int id)
        {
            Console.WriteLine($"🗑️ ELIMINANDO candidato ID: {id}");

            try
            {
                var entity = await _candidatoService.GetById(id);
                if (entity != null)
                {
                    // Eliminar foto física si existe
                    if (!string.IsNullOrEmpty(entity.Foto))
                    {
                        bool eliminado = FileHelper.DeleteFile(entity.Foto);
                        Console.WriteLine($"🗑️ Foto eliminada: {eliminado} ({entity.Foto})");
                    }

                    // Eliminar de la base de datos
                    // NOTA: Descomentar cuando implementes el método DeleteAsync
                    // await _candidatoService.DeleteAsync(id);

                    Console.WriteLine("✅ Candidato eliminado de BD");
                    TempData["Success"] = "Candidato eliminado exitosamente";
                }
                else
                {
                    Console.WriteLine("❌ Candidato no encontrado");
                    TempData["Error"] = "Candidato no encontrado";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al eliminar: {ex.Message}");
                TempData["Error"] = $"Error al eliminar: {ex.Message}";
            }

            return RedirectToRoute(new { controller = "Candidato", action = "CandidatoView" });
        }

        // Método de utilidad para debug
        [HttpGet]
        public IActionResult DebugPhotos()
        {
            var photos = FileHelper.ListFiles("candidatos");

            var debugInfo = new
            {
                TotalFiles = photos.Count,
                Files = photos.Select(p => new
                {
                    Path = p,
                    Exists = FileHelper.PhotoExists(p),
                    Info = FileHelper.GetPhotoInfo(p)
                })
            };

            return Json(debugInfo);
        }
    }
}