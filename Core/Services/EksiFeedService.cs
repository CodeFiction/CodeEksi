using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Server.Services.Helpers;
using Services.Contracts;
using Services.Contracts.Exceptions;
using Services.Contracts.Models;
using HtmlParser = AngleSharp.Parser.Html.HtmlParser;

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
                .Binder()
                .WithUrl("https://eksisozluk.com/debe")
                .WithQueryString(new KeyValuePair<string, string>("_", DateTime.Now.Ticks.ToString()))
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<DebeTitleModel>(model =>
                {
                    string decodedUrl = WebUtility.UrlDecode(model.Link);
                    string entryId = $"#{decodedUrl.Split('#')[1]}";

                    model.EntryId = entryId;
                });

            return debeTitleModels.ToList();
        }

        public async Task<PopulerModel> GetPopulerList(int? page = null)
        {
            IList<KeyValuePair<string, string>> queryStringParameters = new List<KeyValuePair<string, string>>();

            int currentPage = page ?? 1;

            queryStringParameters.Add(new KeyValuePair<string, string>("_", DateTime.Now.Ticks.ToString()));
            queryStringParameters.Add(new KeyValuePair<string, string>("p", currentPage.ToString()));

            var populerList = await _bindingComponent
                .Binder()
                .WithUrl("https://eksisozluk.com/basliklar/populer")
                .WithQueryString(queryStringParameters.ToArray())
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<PopulerTitleModel>(model =>
                {
                    model.Title = model.Title.Substring(0, model.Title.Length - model.EntryCount.Length);
                });

            List<PopulerTitleModel> populerTitleModels = populerList.ToList();

            // TODO @deniz Geçici kod! binding yapısı nested tipleri handle edecek şekilde değiştirilmeli.

            // Pager ilk sayfada gelmiyor. 2. sayfadan itibaren geliyor.
            PopulerModel populerModel = (await _bindingComponent
                .Binder()
                .WithUrl("https://eksisozluk.com/basliklar/populer")
                .WithQueryString(queryStringParameters.ToArray())
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<PopulerModel>()).FirstOrDefault();

            populerModel = populerModel ?? new PopulerModel() {CurrentPage = "1", PageCount = "2"};

            populerModel.PopulerTitleModels = populerTitleModels;

            return populerModel;
        }

        public async Task<EntryDetailModel> GetEntryById(string entryId)
        {
            IEnumerable<EntryDetailModel> entryDetailModels = await _bindingComponent
                .Binder()
                .WithUrl($"https://eksisozluk.com/entry/{entryId}")
                .WithCssSelectorParameter(new KeyValuePair<string, string>("entryId", entryId))
                .BindModel<EntryDetailModel>(model =>
                {
                    model.EntryId = entryId;
                });

            return entryDetailModels.FirstOrDefault();
        }

        public async Task<SearchResult> Search(string titleText)
        {
            Uri uri = new Uri("https://eksisozluk.com/");

            uri = uri.AddParameter("q", titleText);

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    SearchResult searchResult = new SearchResult();

                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        searchResult.EntryDetailModels = _bindingComponent
                            .Binder()
                            .BindModelWithStream<EntryDetailModel>(stream).ToList();

                        searchResult.Result = true;
                    }

                    searchResult.SuggestedTitles = _bindingComponent
                        .Binder()
                        .BindModelWithStream<SuggestedTitle>(stream).ToList();

                    return searchResult;
                }
            }
        }
    }
}
