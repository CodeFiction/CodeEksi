using System.Collections.Generic;
using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    public class DebeModel
    {
        [DataMember(Name = "page_count")]
        public string PageCount { get; set; }

        [DataMember(Name = "current_page")]
        public string CurrentPage { get; set; }

        [BindModel]
        [DataMember(Name = "debe_title_models")]
        public IList<DebeTitleHeaderModel> DebeTitleHeaderModels { get; set; }
    }
}