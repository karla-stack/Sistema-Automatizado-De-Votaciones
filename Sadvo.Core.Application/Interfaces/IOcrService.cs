

namespace Sadvo.Core.Application.Interfaces
{
    public interface IOcrService
    {
        Task<string> ExtractIdNumberFromImageAsync(byte[] imageBytes);
    }
}
