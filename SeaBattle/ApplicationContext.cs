using System;
using SeaBattle.Controller;
using SeaBattle.Logger;
using Spectre.Console;
using Environment = SeaBattle.Config.Environment;

namespace SeaBattle
{
    class ApplicationContext
    {
        private static readonly SBLogger log = SBLoggerFactory.GetLogger<ApplicationContext>();
        private Environment Environment { get; }

        public ApplicationContext()
        {
            Environment = new Environment();
        }

        public void StartGame()
        {
            var viewResolver = Environment.GetComponent<ViewResolver>();
            string currentView = "Start";
            while (currentView is not null)
            {
                try
                {
                    currentView = viewResolver.GetView(currentView)?.Render();
                }
                catch (Exception ex)
                {
                    log.Error("Unexpected exception", ex);
                    AnsiConsole.MarkupLine("[red] Something went WRONG :([/]");

                    System.Environment.Exit(-1);
                }
            }
        }
    }
}
