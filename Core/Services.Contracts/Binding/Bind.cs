using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Binding
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class Bind : Attribute
    {
        private readonly string _cssSelector;

        public Bind(string cssSelector)
        {
            _cssSelector = cssSelector;
        }

        public string AttributeName { get; set; }

        public bool InnerText { get; set; }
    }
}
