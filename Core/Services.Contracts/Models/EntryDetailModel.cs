using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("li[data-id={entryId}]")]
    public class EntryDetailModel
    {
        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }

        [Bind("div", InnerText = true)]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [Bind("footer > div.info > a.entry-date ", InnerText = true)]
        [DataMember(Name = "date")]
        public string EntryDate { get; set; }

        [Bind(null, AttributeName = "data-author")]
        [DataMember(Name = "author")]
        public string EntryAuthor { get; set; }
    }
}
