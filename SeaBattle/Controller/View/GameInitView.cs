using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SeaBattle.Config;
using SeaBattle.Logger;
using SeaBattle.Model;
using SeaBattle.Model.Dto;
using SeaBattle.Service;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    class GameInitView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<GameInitView>();
        private readonly AppCache appCache;
        private readonly GameService gameService;
        private readonly UserService userService;
        private readonly GameProcessService gameProcessService;
        private readonly BattleFieldRenderHelper battleFieldRenderHelper;
        private readonly UserSignInfoProvider userSignInfoProvider;
        private readonly CoordinateProvider coordinateProvider;

        public GameInitView(
            AppCache appCache,
            GameService gameService,
            UserService userService,
            GameProcessService gameProcessService,
            BattleFieldRenderHelper battleFieldRenderHelper,
            UserSignInfoProvider userSignInfoProvider,
            CoordinateProvider coordinateProvider
        ) : base(new[] { nameof(GameProcessView) })

        {
            this.appCache = appCache;
            this.gameService = gameService;
            this.userService = userService;
            this.gameProcessService = gameProcessService;
            this.battleFieldRenderHelper = battleFieldRenderHelper;
            this.userSignInfoProvider = userSignInfoProvider;
            this.coordinateProvider = coordinateProvider;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(GameInitView)}");
            if (appCache.NextGameId.HasValue)
            {
                appCache.CurrentGame = ContinueGame(appCache.NextGameId.Value, appCache.Admin, out var opponent);
                appCache.Opponent = opponent;
            }
            else
            {
                appCache.Opponent ??= LoginOpponent();
                appCache.CurrentGame = gameProcessService.CreateNewGame(appCache.Admin, appCache.Opponent);
            }

            var isReady = gameProcessService.IsGameReady(appCache.CurrentGame, out var unready);

            if (isReady)
                return;

            foreach (var uBf in unready)
            {
                ReRenderHeader();
                while (!AnsiConsole.Confirm($"{uBf.BattleField.Owner.Name} ready?"))
                {
                    //wait for ready
                }
                FillBattleField(uBf);
            }
        }

        private GameDto ContinueGame(ulong gameId, UserDto admin, out UserDto opponent)
        {
            var game = gameProcessService.LoadGame(gameId);
            var expectedOpponent = game.BattleFields.First(bf => bf.Owner.Id != admin.Id).Owner;
            opponent = null;
            while (opponent is null)
            {
                AnsiConsole.MarkupLine($"Awaiting {expectedOpponent.Name} to log in");
                log.Debug($"Expecting user Id:{expectedOpponent.Id} to enter the game Id:{gameId}");
                var login = userSignInfoProvider.SignInInfo(out var password);
                if (gameService.CheckLoginForGameId(gameId, login))
                {
                    opponent = userService.GetUserByLoginAndPassword(login, password);
                    if (opponent is null || opponent.Id.Equals(appCache.Admin.Id))
                    {
                        AnsiConsole.MarkupLine("[red]Wrong password.[/]");
                        log.Info("Incorrect password input ");
                        opponent = null;
                        continue;
                    }

                    return game;
                }
                AnsiConsole.MarkupLine("[red]This user doesn't match the game[/]");
                log.Info("User doesn't match the game");
            }

            return game;
        }

        private UserDto LoginOpponent()
        {
            var choose = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("There is no opponent")
                 .AddChoices("Sign in", "Sign up"));
            log.Info($"User choose:{choose}");
            string login;
            string password;
            if (choose.Equals("Sign in"))
            {
                UserDto user = null;
                while (user is null)
                {
                    login = userSignInfoProvider.SignInInfo(out password);
                    user = userService.GetUserByLoginAndPassword(login, password);
                    if (user is null)
                    {
                        AnsiConsole.MarkupLine("[red]User not found. Check login or password.[/]");
                        log.Info("Incorrect login/password input");
                    }

                    if (user?.Id.Equals(appCache.Admin.Id) ?? false)
                    {
                        AnsiConsole.MarkupLine("[red]You can’t play with yourself.[/]");
                        user = null;
                    }
                }

                return user;
            }

            login = userSignInfoProvider.SignUpInfo(out password, out var name);
            return userService.SaveUser(new UserDto(name, login, password));

        }

        private void FillBattleField(GameProcessService.UnreadyField uBf)
        {
            log.Debug("Filling the battlefield");
            var rest = uBf.RestToPlace;
            bool isReady = false;
            while (!isReady)
            {
                UpdateViewState(uBf.BattleField, rest);
                bool isShipAdded = false;
                while (!isShipAdded)
                {
                    var shipType = ReadPlaceShip(rest, out var rotation, out var x, out var y);
                    isShipAdded = gameProcessService.AddShip(uBf.BattleField, shipType, x, y, rotation);
                    AnsiConsole.MarkupLine(isShipAdded ? "[green]Ship added successfully[/]" : "[red]Can't place ship here[/]");
                    Thread.Sleep(1_000);
                }

                gameProcessService.SaveGame(appCache.CurrentGame);
                isReady = gameProcessService.IsBattleFieldReady(uBf.BattleField, out rest);
            }
        }

        private void UpdateViewState(BattleFieldDto battleField, Dictionary<int, int> rest)
        {
            string haveToPlace = default;
            foreach (var (deck, count) in rest)
            {
                haveToPlace += $"\t{count} ships with {deck} decks\n";
            }

            ReRenderHeader();
            AnsiConsole.Render(new Markup($"{battleField.Owner?.Name}. Have to place: \n{haveToPlace}\n\n"));

            AnsiConsole.Render(battleFieldRenderHelper.CreateField(battleField, false));
            AnsiConsole.MarkupLine("");
        }

        private ShipType ReadPlaceShip(Dictionary<int, int> rest, out ShipRotation rotation, out int x, out int y)
        {
            var typeChoose = AnsiConsole.Prompt(
                new SelectionPrompt<int>()
                    .Title("What place next?")
                    .AddChoices(rest.Keys.OrderBy(k => k))
                    );
            AnsiConsole.MarkupLine($"Place [yellow]{typeChoose} decker[/]");
            var shipType = typeChoose switch
            {
                1 => ShipType.OneDecker,
                2 => ShipType.TwoDecker,
                3 => ShipType.ThreeDecker,
                4 => ShipType.FourDecker
            };
            log.Debug($"User choose {typeChoose} deck ship");
            var original = coordinateProvider.ReadCoordinates("Where to place?", out x, out y);
            log.Debug($"User choose {x}{y} position");
            var orientationChoose = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What orientation?")
                    .AddChoices("Up", "Down", "Left", "Right")
            );
            log.Debug($"User choose {orientationChoose} rotation");

            if (typeChoose == 1)
            {
                rotation = ShipRotation.Up;
                return shipType;
            }

            rotation = orientationChoose switch
            {
                "Up" => ShipRotation.Up,
                "Down" => ShipRotation.Down,
                "Left" => ShipRotation.Left,
                "Right" => ShipRotation.Right
            };

            return shipType;
        }
    }
}
