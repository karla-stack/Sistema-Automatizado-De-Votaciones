
using Sadvo.Core.Application.ViewModels.Usuarios;
using Sadvo.Core.Application.Helpers;
using Sadvo.Core.Application.Interfaces;

namespace SadvoWebApp.Middleware
{
    public class ValidateUserSession : IValidateUserSession
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ValidateUserSession(IHttpContextAccessor httpContextAccessor)
        {
            this._contextAccessor = httpContextAccessor;
        }

        public UserViewModel? GetUserSession()
        {
            UserViewModel? userViewModel = _contextAccessor.HttpContext?.Session.Get<UserViewModel>("User");

            if (userViewModel == null)
            {
                return null;
            }

            return userViewModel;

        }


        public bool HasUserSession()
        {
            UserViewModel? userViewModel = _contextAccessor.HttpContext?.Session.Get<UserViewModel>("User");

            if (userViewModel == null)
            {
                return false;
            }

            return true;

        }
    }
}
