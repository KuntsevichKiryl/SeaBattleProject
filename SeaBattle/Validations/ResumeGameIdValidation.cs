using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Validations
{
    class ResumeGameIdValidation
    {
        private readonly GameService gameService;

        public ResumeGameIdValidation(GameService gameService)
        {
            this.gameService = gameService;
        }

        public ValidationResult IsValid(ulong gameId, ulong userId)
        {
            if (!gameService.CheckGameByUserId(gameId, userId))
            {
                return ValidationResult.Error("Invalid game Id");
            }

            return ValidationResult.Success();
        }
    }
}
