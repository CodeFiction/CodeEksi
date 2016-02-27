using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class DebeTitleModel : BaseTitleModel
    {
        [DataMember(Name = "entry_id")]
        public string EntryId { get; set; }
    }
}