using SeaBattle.Attributes;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    [ViewName("Sign In")]
    class LoginView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<LoginView>();
        private readonly UserService userService;
        private readonly AppCache appCache;
        private readonly UserSignInfoProvider userSignInfoProvider;

        public LoginView(UserService userService, AppCache appCache, UserSignInfoProvider userSignInfoProvider) : base(new[] { "redirectOnSuccess" })
        {
            this.userService = userService;
            this.appCache = appCache;
            this.userSignInfoProvider = userSignInfoProvider;
            ChoseCallback = Redirect;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(LoginView)}");
            UserDto user = null;
            while (user is null)
            {
                var login = userSignInfoProvider.SignInInfo(out var password);
                user = userService.GetUserByLoginAndPassword(login, password);
                if (user is null)
                {
                    AnsiConsole.Render(new Markup("[red]User not found. Check login or password.[/]\n"));
                    log.Info("Incorrect login/password input");
                }
            }
            appCache.Admin = user;
        }

        private string Redirect(string redirectOnSuccess)
        {
            return nameof(UserAccountView);
        }
    }
}
