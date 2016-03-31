using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("ul.topic-list.partial > li > a")]
    public class PopulerTitleHeaderModel
    {
        [Bind(null, ElementValueSelector = ElementValueSelector.InnerText)]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [Bind(null, AttributeName = "href")]
        [DataMember(Name = "link")]
        public string Link { get; set; }

        [Bind("small", ElementValueSelector = ElementValueSelector.InnerText)]
        [DataMember(Name = "entry_count")]
        public string EntryCount { get; set; }
    }
}