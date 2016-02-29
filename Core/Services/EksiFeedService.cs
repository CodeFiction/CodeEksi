using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Services.Contracts;
using Services.Contracts.Models;

namespace Server.Services
{
    public class EskiFeedService : IEksiFeedService
    {
        private readonly IBindingComponent _bindingComponent;

        public EskiFeedService(IBindingComponent bindingComponent)
        {
            _bindingComponent = bindingComponent;
        }

        public async Task<IList<DebeTitleModel>> GetDebeList()
        {
            var debeTitleModels = await _bindingComponent
                .Bind()
                .WithUrl("https://eksisozluk.com/debe?_=")
                .WithRandomQueryString()
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<DebeTitleModel>(model =>
                {
                    string decodedUrl = WebUtility.UrlDecode(model.Link);
                    string entryId = $"#{decodedUrl.Split('#')[1]}";

                    model.EntryId = entryId;
                });

            return debeTitleModels.ToList();
        }

        public async Task<IList<PopulerTitleModel>> GetPopulerList()
        {
            var populerList = await _bindingComponent
                .Bind()
                .WithUrl("https://eksisozluk.com/populer?_=")
                .WithRandomQueryString()
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<PopulerTitleModel>(model =>
                {
                    model.Title = model.Title.Substring(0, model.Title.Length - model.EntryCount.Length);
                });

            return populerList.ToList();
        }

        public async Task<EntryDetailModel> GetEntryById(string entryId)
        {
            IEnumerable<EntryDetailModel> entryDetailModels = await _bindingComponent
                .Bind()
                .WithUrl($"https://eksisozluk.com/entry/{entryId}")
                .WithCssSelectorParameter(new KeyValuePair<string, string>("entryId", entryId))
                .BindModel<EntryDetailModel>(model =>
                {
                    model.EntryId = entryId;
                });

            return entryDetailModels.FirstOrDefault();
        }
    }
}
