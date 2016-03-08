using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.Models
{
    public class SearchResult
    {
        public bool Result { get; set; }

        public IList<SuggestedTitle> SuggestedTitles { get; set; }

        public IList<EntryDetailModel> EntryDetailModels { get; set; }

        public string Title { get; set; }
    }
}
