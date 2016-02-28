using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class EntryDetailModel
    {
        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }

        [DataMember(Name = "entry_content")]
        public string Content { get; set; }

        [DataMember(Name = "entry_date")]
        public string EntryDate { get; set; }

        [DataMember(Name = "entry_writer")]
        public string EntryWriter { get; set; }


    }
}
