using System;

namespace SeaBattle.Exceptions
{
    class UserAlreadyExists : Exception
    {
        public UserAlreadyExists(string message) : base(message) 
        {
        }
    }
}
