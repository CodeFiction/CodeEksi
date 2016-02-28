using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public abstract class BaseTitleModel
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}
