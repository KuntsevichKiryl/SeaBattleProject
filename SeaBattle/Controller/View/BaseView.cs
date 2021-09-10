using SeaBattle.Logger;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    abstract class BaseView : IView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<BaseView>();
        protected delegate string Callback(string choose);
        private readonly FigletText Header = new FigletText("BattleShips")
            .LeftAligned()
            .Color(Color.Red);
        private readonly Rule Owner = new Rule("From the BEST developer in the world!").RightAligned();
        protected string[] MenuOptions { get; }
        protected Callback ChoseCallback { get; set; }

        protected BaseView(string[] menuOptions)
        {
            MenuOptions = menuOptions;
        }

        protected abstract void Body();

        public string Render()
        {
            ReRenderHeader();

            Body();
            string choose;
            if (MenuOptions.Length == 1)
            {
                choose = MenuOptions[0];
                log.Info($"Redirect to: {choose}");
                return ChoseCallback?.Invoke(choose) ?? choose;
            }
            choose = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(MenuOptions));
            log.Info($"User choice: {choose}");
            return ChoseCallback?.Invoke(choose) ?? choose;
        }

        protected void ReRenderHeader()
        {
            AnsiConsole.Clear();
            AnsiConsole.Render(Header);
            AnsiConsole.Render(Owner);
        }
    }
}
