using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Binding
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingSource : Attribute
    {
        public BindingSource()
        {
        }

        public string Url { get; set; }
    }
}
