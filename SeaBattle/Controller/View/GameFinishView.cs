using SeaBattle.Config;
using SeaBattle.Logger;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    class GameFinishView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameFinishView>();
        private readonly AppCache appCache;
        private readonly BattleFieldRenderHelper battleFieldRenderHelper;

        public GameFinishView(AppCache appCache, BattleFieldRenderHelper battleFieldRenderHelper) : base(new[] { "Main menu", "Replay", "Exit" })
        {
            this.appCache = appCache;
            this.battleFieldRenderHelper = battleFieldRenderHelper;
            ChoseCallback = Redirect;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(GameFinishView)}");
            AnsiConsole.MarkupLine($"[underline green]Congratulation! {appCache.CurrentGame.Winner.Name} WINS![/]");
            log.Info($"{appCache.CurrentGame.Winner.Id} wins gameId:{appCache.CurrentGame.Id}");
            AnsiConsole.Render(battleFieldRenderHelper.CreateFields(appCache.CurrentGame));
            appCache.CurrentGame = null;
            appCache.NextGameId = null;
        }

        private string Redirect(string choose)
        {
            return choose switch
            {
                "Main menu" => MainMenu(),
                "Replay"=>nameof(GameInitView),
                _ => choose
            };
        }

        private string MainMenu()
        {
            appCache.Opponent = null;
            return nameof(UserAccountView);
        }
    }
}
