using System;
using System.Threading;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    class GameProcessView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameProcessView>();
        private readonly AppCache appCache;
        private readonly BattleFieldRenderHelper battleFieldRenderHelper;
        private readonly GameProcessService gameProcessService;
        private readonly CoordinateProvider coordinateProvider;
        
        public GameProcessView(
            AppCache appCache,
            BattleFieldRenderHelper battleFieldRenderHelper,
            GameProcessService gameProcessService,
            CoordinateProvider coordinateProvider
            ) : base(new[] { nameof(GameFinishView) })
        {
            this.appCache = appCache;
            this.battleFieldRenderHelper = battleFieldRenderHelper;
            this.gameProcessService = gameProcessService;
            this.coordinateProvider = coordinateProvider;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(GameProcessView)}");
            var style = new Style(Color.Yellow);
            ulong? lastTurn = null;
            while (appCache.CurrentGame.Winner is null)
            {
                if (!lastTurn.HasValue || !lastTurn.Equals(appCache.CurrentGame.CurrentUser.Id))
                {
                    RenderField(false);
                    while (!AnsiConsole.Confirm($"{appCache.CurrentGame.CurrentUser.Name} is ready?"))
                    {
                        //waiting for ready
                    }
                    lastTurn = appCache.CurrentGame.CurrentUser.Id;
                }
                RenderField();

                coordinateProvider.ReadCoordinates(
                    $"{appCache.CurrentGame.CurrentUser.Name}, make your shot.",
                    out var x,
                    out var y,
                    style
                    );

                var result = gameProcessService.AddShot(appCache.CurrentGame, x, y);

                gameProcessService.SaveGame(appCache.CurrentGame);
                switch (result)
                {
                    case ShotResult.Destroy:
                        AnsiConsole.MarkupLine("[green]Ship Destroyed![/]");
                        break;
                    case ShotResult.Hit:
                        AnsiConsole.MarkupLine("[green]Hit![/]");
                        break;
                    case ShotResult.Miss:
                        AnsiConsole.MarkupLine("[yellow]Miss...[/]");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(result));
                }
                Thread.Sleep(1_000);
            }
        }

        private void RenderField(bool showField = true)
        {
            ReRenderHeader();
            AnsiConsole.Render(new Markup($"{appCache.Admin.Name} vs {appCache.Opponent.Name}\nLet the BATTLE BEGIN!\n"));
            AnsiConsole.Render(new Markup($"{appCache.CurrentGame.CurrentUser.Name} turn.\n"));
            if (showField)
            {
                AnsiConsole.Render(battleFieldRenderHelper.CreateFields(appCache.CurrentGame));
            }
        }
    }
}
