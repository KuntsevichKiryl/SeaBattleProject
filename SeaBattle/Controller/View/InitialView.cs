using SeaBattle.Attributes;
using SeaBattle.Logger;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    [ViewName("Start")]
    class InitialView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<InitialView>();
        public InitialView() : base(new[] { "Sign Up", "Sign In", "Exit" })
        {
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(InitialView)}");
            AnsiConsole.Render(new Markup("Welcome!\n"));
            AnsiConsole.Render(new Markup("What next?\n"));
        }
    }
}
