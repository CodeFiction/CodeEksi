using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts.Models;

namespace Services.Contracts
{
    public interface IEksiFeedService
    {
        Task<IList<DebeTitleModel>> GetDebeList();
        Task<PopulerModel> GetPopulerList(int? page = null);
        Task<EntryDetailModel> GetEntryById(string entryId);
        Task<SearchResult> Search(string titleText);
    }
}