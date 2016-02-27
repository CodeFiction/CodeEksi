using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class PopulerTitleModel : BaseTitleModel
    {
        [DataMember(Name = "entry_count")]
        public int EntryCount { get; set; }
    }
}