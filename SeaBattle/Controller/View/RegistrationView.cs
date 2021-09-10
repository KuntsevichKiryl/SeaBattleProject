using SeaBattle.Attributes;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;
using SeaBattle.Service;

namespace SeaBattle.Controller.View
{
    [ViewName("Sign Up")]
    class RegistrationView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<RegistrationView>();
        private readonly UserService userService;
        private readonly AppCache appCache;
        private readonly UserSignInfoProvider userSignInfoProvider;

        public RegistrationView(
            UserService userService,
            AppCache appCache,
            UserSignInfoProvider userSignInfoProvider
            ) : base(new[] { "redirectOnSuccess" })
        {
            this.userService = userService;
            this.appCache = appCache;
            this.userSignInfoProvider = userSignInfoProvider;
            ChoseCallback = Redirect;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(RegistrationView)}");
            var login = userSignInfoProvider.SignUpInfo(out var password, out var name);
            var user = new UserDto(name, login, password);
            user = userService.SaveUser(user);
            appCache.Admin = user;
        }

        private string Redirect(string redirectOnSuccess)
        {
            return nameof(UserAccountView);
        }
    }
}
