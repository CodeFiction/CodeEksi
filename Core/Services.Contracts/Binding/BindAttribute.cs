using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

        public bool ApplySelectorToHtmlDocument { get; set; }

        public string CssSelector { get; }

        public string AttributeName { get; set; }

        public ElementValueSelector ElementValueSelector { get; set; }
    }
}
