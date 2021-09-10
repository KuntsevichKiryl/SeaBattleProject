using System.Text.RegularExpressions;
using SeaBattle.Controller.View;
using SeaBattle.Logger;
using Spectre.Console;

namespace SeaBattle.Validations
{
    class CoordinateValidation
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<CoordinateValidation>();
        private readonly Regex format = new(@"^^[A-Ja-j]([1-9]|10)$");

        public ValidationResult IsValid(string input)
        {
            log.Debug("Validating coordinates");
            if (input is null || input.Trim().Length == 0)
            {
                return ValidationResult.Error("Should be present");
            }

            if (!format.IsMatch(input))
            {
                return ValidationResult.Error("Format violation. Should starts with latin letter between A and J of any register and digit between 1 and 10.\nExamples:\n\tA1\n\tj10");
            }

            return ValidationResult.Success();
        }
    }
}
