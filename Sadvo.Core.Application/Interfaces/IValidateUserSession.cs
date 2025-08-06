
using Sadvo.Core.Application.ViewModels.Usuarios;

namespace Sadvo.Core.Application.Interfaces
{
    public interface IValidateUserSession
    {
        UserViewModel? GetUserSession();
        bool HasUserSession();
    }
}
