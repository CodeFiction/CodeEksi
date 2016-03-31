using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Exceptions
{
    public class BindAttributeNotFoundException : UserLevelException
    {
        public BindAttributeNotFoundException(string message) : base(message)
        {
        }
    }
}
