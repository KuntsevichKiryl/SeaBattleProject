using System;
using SeaBattle.Attributes;
using SeaBattle.Config;
using SeaBattle.Logger;
using Spectre.Console;

namespace SeaBattle.Controller.View
{
    [ViewName("Exit")]
    class ExitView : BaseView
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<ExitView>();
        private readonly AppCache appCache;

        public ExitView(AppCache appCache) : base(Array.Empty<string>())
        {
            this.appCache = appCache;
        }

        protected override void Body()
        {
            log.Debug($"Render {nameof(ExitView)}");
            string message = "Bye!\n";
            if (appCache.Admin is not null)
            {
                message = message + " " + appCache.Admin.Name;
            }
            AnsiConsole.Render(new Markup(message));
            log.Info("Exit application");
            System.Environment.Exit(0);
        }
    }
}
