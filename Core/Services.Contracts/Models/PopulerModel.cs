using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("div.pager")]
    public class PopulerModel
    {
        [Bind(null, AttributeName = "data-currentpage")]
        [DataMember(Name = "page_count")]
        public string PageCount { get; set; }

        [Bind(null, AttributeName = "data-pagecount")] // TODO : optinal binding desteği getirilmeli.
        [DataMember(Name = "current_page")]
        public string CurrentPage { get; set; }

        [BindModel]
        [DataMember(Name = "populer_title_models")] 
        public IList<PopulerTitleHeaderModel> PopulerTitleHeaderModels { get; set; }
    }
}
