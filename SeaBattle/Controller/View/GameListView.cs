using System.Collections.Generic;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model.Dto;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    class GameListView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameListView>();
        private readonly GameService gameService;
        private readonly AppCache appCache;
        
        public GameListView(GameService gameService, AppCache appCache) : base(new[] { "Back", "Exit" })
        {
            this.gameService = gameService;
            this.appCache = appCache;
            ChoseCallback = Redirect;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(GameListView)}");
            List<GameDto> games = gameService.GetGamesByUserId(appCache.Admin.Id.Value, 0);
            var table = new Table();

            table.AddColumn("Game Id");
            table.AddColumn("Opponent");
            table.AddColumn("Result");
            table.AddColumn("Date");

            foreach (var game in games)
            {
                var adminId = appCache.Admin.Id;
                var enemyBf = game.BattleFields.Find(bf => bf.Owner.Id != adminId);
                var result = game.Winner is null
                    ? "In progress" : game.Winner.Id == adminId
                    ? "Win" : "Lose";
                table.AddRow(game.Id.ToString(), enemyBf?.Owner.Name ?? "", result, game.StartTime.ToString());
            }

            AnsiConsole.Render(table);
        }

        private string Redirect(string choose)
        {
            return choose switch
            {
                "Back" => nameof(UserAccountView),
                _ => choose
            };
        }
    }
}
