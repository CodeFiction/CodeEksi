using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class EntryDetailModel
    {
        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "date")]
        public string EntryDate { get; set; }

        [DataMember(Name = "author")]
        public string EntryAuthor { get; set; }
    }
}
