using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Validations;
using Spectre.Console;

namespace SeaBattle.Controller
{
    class UserSignInfoProvider
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<UserSignInfoProvider>();
        private readonly UserLoginValidation loginValidation;

        public UserSignInfoProvider(UserLoginValidation loginValidation)
        {
            this.loginValidation = loginValidation;
        }

        public string SignUpInfo(out string password, out string name)
        {
            log.Debug("Creating user");
            var loginPrompt = new TextPrompt<string>("Login?");
            loginPrompt.Validator = CheckLoginNotExists;
            string login = AnsiConsole.Prompt(loginPrompt);

            var pwdPrompt = new TextPrompt<string>("Password?").Secret();
            password = AnsiConsole.Prompt(pwdPrompt);

            name = AnsiConsole.Ask<string>("Name?");

            return login;
        }

        public string SignInInfo(out string password)
        {
            log.Debug("Login in user");
            var loginPrompt = new TextPrompt<string>("Login?");
            loginPrompt.Validator = CheckLoginExists;
            string login = AnsiConsole.Prompt(loginPrompt);

            var pwdPrompt = new TextPrompt<string>("Password?").Secret();
            password = AnsiConsole.Prompt(pwdPrompt);

            return login;
        }

        private ValidationResult CheckLoginExists(string login)
        {
            log.Debug($"Checking user {login} existing");
            var result = loginValidation.IsValid(login, true);
            return !result.Successful ? ValidationResult.Error($"[red]{result.Message}[/]") : result;
        }

        private ValidationResult CheckLoginNotExists(string login)
        {
            log.Debug($"Checking user login:{login}  is unique");
            var result = loginValidation.IsValid(login, false);
            return !result.Successful ? ValidationResult.Error($"[red]{result.Message}[/]") : result;
        }
    }
}