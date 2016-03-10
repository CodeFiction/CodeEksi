using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Exceptions
{
    public class CssSelectorNotFoundException : UserLevelException
    {
        public CssSelectorNotFoundException(string message) : base(message)
        {
        }
    }
}
