using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

public static class FileHelper
{
    // Configuraciones por defecto
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static readonly long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB


    public static async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                Console.WriteLine(" No se recibió archivo o está vacío");
                return null;
            }

            Console.WriteLine($" Procesando archivo: {file.FileName} ({file.Length} bytes)");

            // Validar archivo
            string validationError = ValidateFile(file);
            if (validationError != null)
            {
                Console.WriteLine($" Error de validación: {validationError}");
                throw new Exception(validationError);
            }

            // Asegurar que existe la estructura de carpetas
            EnsureUploadFoldersExist();

            // Obtener ruta de uploads
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(wwwrootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                Console.WriteLine($"📁 Carpeta creada: {uploadsFolder}");
            }

            // Generar nombre único
            string uniqueFileName = GenerateUniqueFileName(file.FileName);
            string fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            Console.WriteLine($"💾 Guardando en: {fullFilePath}");

            // Guardar archivo físicamente
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            if (File.Exists(fullFilePath))
            {
                var fileInfo = new FileInfo(fullFilePath);
                Console.WriteLine($"✅ Archivo guardado exitosamente:");
                Console.WriteLine($"   - Tamaño: {fileInfo.Length} bytes");
                Console.WriteLine($"   - Fecha: {fileInfo.CreationTime}");

                // Retornar ruta relativa para la BD
                string relativePath = $"/uploads/{folderName}/{uniqueFileName}";
                Console.WriteLine($"🔗 Ruta para BD: {relativePath}");
                return relativePath;
            }
            else
            {
                Console.WriteLine("❌ ERROR: El archivo no se guardó");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR en upload: {ex.Message}");
            return null;
        }
    }



    public static string GetPhotoUrl(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        // Si ya es una ruta completa, devolverla tal como está
        if (relativePath.StartsWith("http") || relativePath.StartsWith("/"))
            return relativePath;

        // Si no tiene la barra inicial, agregarla
        return relativePath.StartsWith("/") ? relativePath : $"/{relativePath}";
    }


    public static bool PhotoExists(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return false;

        try
        {
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullPath = Path.Combine(wwwrootPath, relativePath.TrimStart('/'));
            return File.Exists(fullPath);
        }
        catch
        {
            return false;
        }
    }


    public static FileInfo GetPhotoInfo(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;

        try
        {
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullPath = Path.Combine(wwwrootPath, relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
                return new FileInfo(fullPath);

            return null;
        }
        catch
        {
            return null;
        }
    }


    public static string GeneratePhotoHtml(string relativePath, string altText = "", string cssClass = "", string style = "", bool showFallback = true)
    {
        if (!string.IsNullOrEmpty(relativePath) && PhotoExists(relativePath))
        {
            return $@"<img src=""{GetPhotoUrl(relativePath)}"" alt=""{altText}"" class=""{cssClass}"" style=""{style}"">";
        }
        else if (showFallback)
        {
            return $@"<div class=""bg-secondary d-flex align-items-center justify-content-center {cssClass}"" style=""{style}"">
                        <i class=""fas fa-user text-white""></i>
                      </div>";
        }

        return "";
    }

    public static bool DeleteFile(string relativePath)
    {
        try
        {
            if (string.IsNullOrEmpty(relativePath))
                return true;

            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string fullPath = Path.Combine(wwwrootPath, relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Console.WriteLine($"🗑️ Archivo eliminado: {fullPath}");
            }
            else
            {
                Console.WriteLine($"ℹ️ Archivo no existía: {fullPath}");
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR eliminando archivo: {ex.Message}");
            return false;
        }
    }

    public static string ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return "No se ha seleccionado archivo";

        // Validar tamaño
        if (file.Length > MaxFileSizeBytes)
        {
            var maxSizeMB = MaxFileSizeBytes / (1024.0 * 1024.0);
            return $"El archivo excede el tamaño máximo de {maxSizeMB:F1}MB";
        }

        // Validar extensión
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
        {
            var allowedExts = string.Join(", ", AllowedImageExtensions);
            return $"Extensión no permitida. Permitidas: {allowedExts}";
        }

        return null; // Archivo válido
    }


    public static bool IsValidImage(IFormFile file)
    {
        return ValidateFile(file) == null;
    }



    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var randomId = Guid.NewGuid().ToString("N")[..8];

        return $"file_{timestamp}_{randomId}{extension}";
    }

    public static void EnsureUploadFoldersExist()
    {
        string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var folders = new[]
        {
            Path.Combine(wwwrootPath, "uploads"),
            Path.Combine(wwwrootPath, "uploads", "candidatos"),
            Path.Combine(wwwrootPath, "uploads", "partidos"),
            Path.Combine(wwwrootPath, "uploads", "noticias"),
            Path.Combine(wwwrootPath, "uploads", "documentos")
        };

        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Console.WriteLine($"📁 Carpeta creada: {folder}");
            }
        }
    }

    public static string FormatFileSize(long bytes)
    {
        if (bytes == 0) return "0 Bytes";

        string[] sizes = { "Bytes", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    public static List<string> ListFiles(string folderName)
    {
        try
        {
            string wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string uploadsFolder = Path.Combine(wwwrootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
                return new List<string>();

            return Directory.GetFiles(uploadsFolder)
                           .Select(f => $"/uploads/{folderName}/{Path.GetFileName(f)}")
                           .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

}

// Extensiones para facilitar el uso
public static class FileHelperExtensions
{ 
    public static async Task<string> SaveToServerAsync(this IFormFile file, string folder)
    {
        return await FileHelper.UploadFileAsync(file, folder);
    }

    public static bool IsValid(this IFormFile file, out string errorMessage)
    {
        errorMessage = FileHelper.ValidateFile(file);
        return errorMessage == null;
    }

    public static string ToPhotoUrl(this string relativePath)
    {
        return FileHelper.GetPhotoUrl(relativePath);
    }

    public static bool FileExists(this string relativePath)
    {
        return FileHelper.PhotoExists(relativePath);
    }
}