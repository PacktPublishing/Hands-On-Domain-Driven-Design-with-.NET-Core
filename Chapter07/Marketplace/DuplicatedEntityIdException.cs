using System;

namespace Marketplace
{
    public class DuplicatedEntityIdException : Exception
    {
        public DuplicatedEntityIdException(string message)
            : base(message)
        {
        }
    }
}