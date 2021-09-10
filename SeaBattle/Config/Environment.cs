using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SeaBattle.Controller;
using SeaBattle.Controller.View;
using SeaBattle.Model.Mapper;
using SeaBattle.Repository;
using SeaBattle.Service;
using SeaBattle.Validations;

namespace SeaBattle.Config
{
    public class Environment
    {
        public ServiceProvider provider = new ServiceCollection()
            .AddDbContext<ApplicationDbContext>()
            .AddSingleton<Environment>()
            .AddSingleton<AppCache>()
            .AddSingleton<UserMapper>()
            .AddSingleton<BattleFieldMapper>()
            .AddSingleton<GameMapper>()
            .AddSingleton<UserService>()
            .AddSingleton<GameService>()
            .AddSingleton<GameProcessService>()
            .AddSingleton<BattleFieldService>()
            .AddSingleton<UserLoginValidation>()
            .AddSingleton<CoordinateValidation>()
            .AddSingleton<ResumeGameIdValidation>()
            .AddSingleton<ViewResolver>()
            .AddSingleton<UserSignInfoProvider>()
            .AddSingleton<CoordinateProvider>()
            .AddSingleton<BattleFieldRenderHelper>()
            .AddSingleton<IView, RegistrationView>()
            .AddSingleton<IView, InitialView>()
            .AddSingleton<IView, LoginView>()
            .AddSingleton<IView, ExitView>()
            .AddSingleton<IView, UserAccountView>()
            .AddSingleton<IView, GameInitView>()
            .AddSingleton<IView, GameProcessView>()
            .AddSingleton<IView, GameFinishView>()
            .AddSingleton<IView, GameListView>()
            .BuildServiceProvider();
        
        public T GetComponent<T>()
        {
            var type = typeof(T);
            return (T)provider.GetRequiredService(type);
        }

        public IEnumerable<T> GetComponents<T>()
        {
            var type = typeof(T);
            return (IEnumerable<T>)provider.GetServices(type);
        }
    }
}
