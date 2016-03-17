using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Contracts.Models;

namespace Services.Contracts
{
    public interface IEksiFeedService
    {
        Task<DebeModel> GetDebeList();
        Task<PopulerModel> GetPopulerList(int? page = null);
        Task<EntryDetailModel> GetEntryById(string entryId);
        Task<SearchResultModel> SearchTitle(string titleText);
        Task<TitleModel> GetTitle(string titleNameIdText, bool? populer = null, int? page = null);
    }
}