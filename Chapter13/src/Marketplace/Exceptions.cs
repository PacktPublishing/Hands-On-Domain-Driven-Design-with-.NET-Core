using System;

namespace Marketplace
{
    public static class Exceptions
    {
        public class DuplicatedEntityIdException : Exception
        {
            public DuplicatedEntityIdException(string message)
                : base(message) { }
        }
    }
}