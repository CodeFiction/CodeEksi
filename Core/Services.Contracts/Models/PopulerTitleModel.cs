using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class PopulerTitleModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "entry_count")]
        public int EntryCount { get; set; }
    }
}