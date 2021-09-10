using SeaBattle.Validations;
using Spectre.Console;

namespace SeaBattle.Controller
{
    class CoordinateProvider
    {
        private readonly CoordinateValidation validation;

        public record Point(int X, int Y);

        public CoordinateProvider(CoordinateValidation validation)
        {
            this.validation = validation;
        }

        public string ReadCoordinates(string message, out int x, out int y, Style style=null)
        {
            var prompt = new TextPrompt<string>(message)
            {
                Validator = validation.IsValid,
                PromptStyle = style ?? new Style()
            };
            var input = AnsiConsole.Prompt(prompt);

            var coord = input.ToUpper().Trim();
            x = coord[0] - 'A';
            y = int.Parse(coord.Substring(1)) - 1;
            return input;
        }
    }
}
