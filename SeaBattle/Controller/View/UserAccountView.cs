using System.Collections.Generic;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;
using SeaBattle.Service;
using SeaBattle.Validations;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    class UserAccountView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<UserAccountView>();
        private readonly GameService gameService;
        private readonly AppCache appCache;
        private readonly ResumeGameIdValidation resumeGameIdValidation;

        public UserAccountView(
            GameService gameService,
            AppCache appCache,
            ResumeGameIdValidation resumeGameIdValidation
            ) : base(new[] { "New Game", "Resume Game", "Full Games List", "Exit" })
        {
            this.gameService = gameService;
            this.appCache = appCache;
            this.resumeGameIdValidation = resumeGameIdValidation;
            ChoseCallback = CallbackAndRedirect;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(UserAccountView)}");
            var root = new Tree($"[orange1]{appCache.Admin.Name}[/]");
            var lastGames = root.AddNode("[green]Last 5 games.[/]");
            lastGames.AddNode(LastGamesTable(5));
            var statistics = root.AddNode("[green]Statistics.[/]");
            statistics.AddNode("Coming soon");
            AnsiConsole.Render(root);
        }

        private Table LastGamesTable(int tableSize)
        {
            List<GameDto> games = gameService.GetGamesByUserId(appCache.Admin.Id.Value, tableSize);
            var table = new Table();

            table.AddColumns("Game Id", "Opponent", "Result", "Date");

            foreach (var game in games)
            {
                var adminId = appCache.Admin.Id;
                var enemyBf = game.BattleFields.Find(bf => (bf.Owner?.Id ?? adminId) != adminId);
                var result = game.Winner is null
                    ? "In progress" : game.Winner.Id == adminId
                        ? "Win" : "Lose";
                table.AddRow(game.Id.ToString(), enemyBf?.Owner.Name ?? "", result, game.StartTime.ToString());
            }
            log.Info("Rendering last games table");
            return table;
        }

        private string CallbackAndRedirect(string chose)
        {
            return chose switch
            {
                "New Game" => NewGame(),
                "Full Games List" => nameof(GameListView),
                "Resume Game" => LoadGame(),
                _ => chose
            };
        }

        private string NewGame()
        {
            appCache.NextGameId = null;
            return nameof(GameInitView);
        }

        private string LoadGame()
        {
            var gameId = new TextPrompt<ulong>("Game Id?");
            gameId.Validator = ValidatorAdapter;
            appCache.NextGameId = AnsiConsole.Prompt(gameId);
            log.Debug($"Loading game id{gameId}");
            return nameof(GameInitView);
        }

        private ValidationResult ValidatorAdapter(ulong gameId)
        {
            return resumeGameIdValidation.IsValid(gameId, appCache.Admin.Id.Value);
        }
    }
}
