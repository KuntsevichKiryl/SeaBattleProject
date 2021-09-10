using System;

namespace SeaBattle.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    class ViewNameAttribute : Attribute
    {
        public string Name { get; }

        public ViewNameAttribute( string name)
        {
            Name = name;
        }
    }
}
