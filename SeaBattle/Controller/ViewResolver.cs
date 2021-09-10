using System;
using System.Collections.Generic;
using System.Reflection;
using SeaBattle.Attributes;
using SeaBattle.Controller.View;
using Environment = SeaBattle.Config.Environment;


namespace SeaBattle.Controller
{
    class ViewResolver
    {
        private readonly Dictionary<string, IView> registeredViews = new();

        public ViewResolver(Environment environment)
        {
            var views = environment.GetComponents<IView>();
            foreach (var view in views)
            {
                Type type = view.GetType();
                var viewNameAttr = (ViewNameAttribute)type.GetCustomAttribute(typeof(ViewNameAttribute));
                if (viewNameAttr?.Name is null)
                {
                    registeredViews.Add(type.Name, view);
                    continue;
                }
                registeredViews.Add(viewNameAttr.Name, view);
            }
        }

        public IView GetView(string name)
        {
            return registeredViews.GetValueOrDefault(name);
        }
    }
}
