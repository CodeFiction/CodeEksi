using System.Collections.Generic;
using System.Runtime.Serialization;
using Services.Contracts.Binding;

namespace Services.Contracts.Models
{
    [DataContract]
    [Bind("div.pager")]
    // TODO : @deniz model'e bind attribute'e zorlamamak lazım. Sadece property yeterli olmalı.
    // PropertyInfo - Func gibi bir ikili tutulabilir. Ama Model'in Bind Att'si yoksa property'ler de css selector zorunlu olmalı
    // Dolayısıyla BindAttribute kullanılırken AttributeName verilmişse css selector null olmalı kuralı revize edilip düzeltilmeli
    public class TitleModel
    {
        [Bind(null, AttributeName = "data-currentpage")]
        [DataMember(Name = "page_count")]
        public string PageCount { get; set; }

        [Bind(null, AttributeName = "data-pagecount")] // TODO : optinal binding desteği getirilmeli.
        [DataMember(Name = "current_page")]
        public string CurrentPage { get; set; }

        [DataMember(Name = "title_name_id_text")]
        public string TitleNameIdText { get; set; }

        [BindModel]
        [DataMember(Name = "entry_detail_models")]
        public IList<EntryDetailModel> EntryDetailModels { get; set; }
    }
}
