using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("ul#entry-list > li")]
    public class EntryDetailModel
    {
        [Bind(null, AttributeName = "data-id")]
        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }

        [Bind("div", ElementValueSelector = ElementValueSelector.InnerHtml)]
        [DataMember(Name = "content")]
        public string Content { get; set; }

        [Bind("footer > div.info > a.entry-date ", ElementValueSelector = ElementValueSelector.InnerText)]
        [DataMember(Name = "date")]
        public string EntryDate { get; set; }

        [Bind(null, AttributeName = "data-author")]
        [DataMember(Name = "author")]
        public string EntryAuthor { get; set; }

        // TODO : @deniz Attribute binder ve element binder gibi iki farklı property olmalı
        [Bind("h1#title > a > span", ApplySelectorToHtmlDocument = true, ElementValueSelector = ElementValueSelector.InnerText)]
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}