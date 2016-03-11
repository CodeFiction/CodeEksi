using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("ul.topic-list.partial > li > a")]
    public class PopulerTitleHeaderModel
    {
        [Bind(null, InnerText = true)]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [Bind(null, AttributeName = "href")]
        [DataMember(Name = "link")]
        public string Link { get; set; }

        [Bind("small", InnerText = true)]
        [DataMember(Name = "entry_count")]
        public string EntryCount { get; set; }
    }
}