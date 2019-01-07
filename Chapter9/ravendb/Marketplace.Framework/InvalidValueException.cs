using System;

namespace Marketplace.Framework
{
    public class InvalidValueException : Exception
    {
        public InvalidValueException(Type type, string message)
            : base($"Value of {type.Name} {message}")
        {
        }
    }
}