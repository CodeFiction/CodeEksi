using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Services.Contracts.Models
{
    [DataContract]
    public class SearchResultModel
    {
        [DataMember(Name = "result")]
        public bool Result { get; set; }

        [DataMember(Name = "suggested_title_models")]
        public IList<SuggestedTitleModel> SuggestedTitleModels { get; set; }

        [DataMember(Name = "title_model")]
        public TitleModel TitleModel { get; set; }
    }
}
