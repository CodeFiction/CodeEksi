using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    // TODO : @deniz tutarlılık açısından Class'lar için ayrı, property'ler için ayrı attribute olmalı.
    [Bind("a.suggested-title")]
    [DataContract]
    public class SuggestedTitleModel
    {
        [Bind(null, ElementValueSelector = ElementValueSelector.InnerText)]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [Bind(null, AttributeName = "href")]
        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}