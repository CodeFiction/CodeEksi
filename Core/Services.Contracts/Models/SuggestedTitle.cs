using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [Bind("a.suggested-title")]
    [DataContract]
    public class SuggestedTitle
    {
        [Bind(null, InnerText = true)]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [Bind(null, AttributeName = "href")]
        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}