using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Binding
{
    [Flags]
    public enum ElementValueSelector : byte
    {
        InnerHtml = 1,
        InnerText = 2
    }
}
