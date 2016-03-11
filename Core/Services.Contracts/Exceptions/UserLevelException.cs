using System;

namespace Services.Contracts.Exceptions
{
    public class UserLevelException : Exception
    {
        public UserLevelException(string message)
            : base(message)
        {            
        }
    }
}