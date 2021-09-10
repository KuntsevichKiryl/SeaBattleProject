using System.Text.RegularExpressions;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Validations
{
    class UserLoginValidation
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<UserLoginValidation>();
        private readonly Regex loginFormat = new(@"^[A-Za-z0-9]{3,15}$");
        private readonly UserService userService;

        public UserLoginValidation(UserService userService)
        {
            this.userService = userService;
        }

        public ValidationResult IsValid(string login, bool shouldExists)
        {
            log.Debug("User login validation");
            if (login is null || login.Trim().Length == 0)
            {
                return ValidationResult.Error("Should be present");
            }

            if (!loginFormat.IsMatch(login))
            {
                return ValidationResult.Error("Format violation. Should contains latin letters of any register or digits. Length should be between 3 and 15 symbols");
            }

            var exists = userService.ExistsByLogin(login);
            if (shouldExists)
            {
                return exists ? ValidationResult.Success() : ValidationResult.Error($"User '{login}' not found");
            }

            return exists ? ValidationResult.Error($"User '{login}' exists") : ValidationResult.Success();
        }
    }
}
