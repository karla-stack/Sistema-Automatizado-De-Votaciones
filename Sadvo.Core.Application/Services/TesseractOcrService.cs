using Sadvo.Core.Application.Interfaces;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;

namespace SADVO.Core.Application.Services
{
    public class TesseractOcrService : IOcrService
    {
        private readonly string _tessDataPath;

        public TesseractOcrService()
        {
            // La ruta a tu carpeta 'tessdata'. Esta debe estar en el directorio de ejecución del proyecto web.
            _tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
        }

        public async Task<string> ExtractIdNumberFromImageAsync(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
            {
                return null;
            }

            try
            {
                // Usamos TesseractEngine para el reconocimiento en español.
                using (var engine = new TesseractEngine(_tessDataPath, "spa", EngineMode.Default))
                {
                    // Tesseract trabaja directamente con los bytes de la imagen.
                    using (var img = Pix.LoadFromMemory(imageBytes))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            // Limpiamos y buscamos el número de cédula en el texto extraído.
                            return await Task.FromResult(FindAndCleanIdNumber(text));
                        }
                    }
                }
            }
            catch (TesseractException ex)
            {
                // En un proyecto real, aquí se registraría el error (log ex).
                // Este error suele ocurrir si no se encuentra la carpeta 'tessdata'.
                return $"Error de OCR: {ex.Message}";
            }
        }

        private string FindAndCleanIdNumber(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return null;
            }

            // Elimina todos los caracteres que no sean dígitos.
            var digitsOnly = Regex.Replace(rawText, @"[^\d]", "");

            // Busca una secuencia de 11 dígitos (formato de cédula dominicana).
            var match = Regex.Match(digitsOnly, @"\d{11}");

            return match.Success ? match.Value : null;
        }
    }
}

