using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Binding
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class BindAttribute : Attribute
    {
        public BindAttribute(string cssSelector)
        {
            CssSelector = cssSelector;
        }


        public string CssSelector { get; }

        public string AttributeName { get; set; }

        public bool InnerText { get; set; }
    }
}
