using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Server.Services.Helpers;
using Server.Services.Utils;
using Services.Contracts;
using Services.Contracts.Exceptions;
using Services.Contracts.Models;
using static System.Threading.Tasks.TaskExtensions;

namespace Server.Services
{
    public class EskiFeedService : IEksiFeedService
    {
        private readonly IBindingComponent _bindingComponent;

        public EskiFeedService(IBindingComponent bindingComponent)
        {
            _bindingComponent = bindingComponent;
        }

        public async Task<DebeModel> GetDebeList()
        {
            DebeModel debeModel = new DebeModel
            {
                CurrentPage = "1",
                PageCount = "1"
            };

            IEnumerable<DebeTitleHeaderModel> debeTitleHeaderModels = await _bindingComponent
                .Binder()
                .WithUrl("https://eksisozluk.com/debe")
                .WithQueryString(new KeyValuePair<string, string>("_", DateTime.Now.Ticks.ToString()))
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<DebeTitleHeaderModel>(model =>
                {
                    string decodedUrl = WebUtility.UrlDecode(model.Link);
                    string entryId = decodedUrl.Split('#')[1];

                    model.EntryId = entryId;
                });

            // TODO : @deniz buradaki işlemin async await mimarisine uygun şekilde BindModel'in içerisinde yapılması gerekiyor.
            var titleHeaderModels = debeTitleHeaderModels as IList<DebeTitleHeaderModel> ?? debeTitleHeaderModels.ToList();

            IEnumerable<Task> entryTasks = titleHeaderModels.Select(model =>
            {
                return RunWithErrorHandling(async () =>
                {
                    EntryDetailModel entryDetailModel = await GetEntryById(model.EntryId);
                    model.DebeEntryDetailModel = entryDetailModel;
                });
            });

            await Task.WhenAll(entryTasks);

            debeModel.DebeTitleHeaderModels = titleHeaderModels.ToList();

            return debeModel;
        }

        public async Task<PopulerModel> GetPopulerList(int? page = null)
        {
            Uri uri = new Uri("https://eksisozluk.com/basliklar/populer");

            int currentPage = page ?? 1;

            uri = uri.AddParameter("_", DateTime.Now.Ticks.ToString())
                .AddParameter("p", currentPage.ToString());

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            return await BindHttpRequestMessage(request, htmlContent =>
            {
                List<PopulerTitleHeaderModel> populerTitleHeaderModels = _bindingComponent
                    .Binder()
                    .BindModelHtmlContent<PopulerTitleHeaderModel>(htmlContent,
                        model =>
                        {
                            model.Title = model.Title.Substring(0, model.Title.Length - model.EntryCount.Length);
                        }).ToList();

                PopulerModel populerModel = _bindingComponent
                    .Binder()
                    .BindModelHtmlContent<PopulerModel>(htmlContent).FirstOrDefault();

                // Pager ilk sayfada gelmiyor. 2. sayfadan itibaren geliyor.
                populerModel = populerModel ?? new PopulerModel() { CurrentPage = "1", PageCount = "2" };

                populerModel.PopulerTitleHeaderModels = populerTitleHeaderModels;

                return populerModel;
            });
        }

        public async Task<EntryDetailModel> GetEntryById(string entryId)
        {
            IEnumerable<EntryDetailModel> entryDetailModels = await _bindingComponent
            .Binder()
            .WithUrl($"https://eksisozluk.com/entry/{entryId}")
            .BindModel<EntryDetailModel>(c => c.Content = c.Content.FixLinks());

            var result = entryDetailModels.FirstOrDefault();
            return result;
        }

        public async Task<SearchResultModel> SearchTitle(string titleText)
        {
            Uri uri = new Uri("https://eksisozluk.com/");

            uri = uri.AddParameter("q", titleText);

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            // TODO : @deniz geçici olarak yapıldı. Nested tipleri handle edilen yapı oluşturulduğunda değiştirilecek
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    string htmlContent = await httpResponseMessage.Content.ReadAsStringAsync();

                    SearchResultModel searchResultModel = new SearchResultModel();

                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string titleNameIdText = httpResponseMessage.RequestMessage.RequestUri.AbsolutePath;

                        TitleModel titleModel = _bindingComponent
                            .Binder()
                            .BindModelHtmlContent<TitleModel>(htmlContent, model => model.TitleNameIdText = titleNameIdText)
                            .FirstOrDefault();

                        titleModel = titleModel ??
                                     new TitleModel()
                                     {
                                         CurrentPage = "1",
                                         PageCount = "1",
                                         TitleNameIdText = titleNameIdText
                                     };

                        titleModel.EntryDetailModels = _bindingComponent
                            .Binder()
                            .BindModelHtmlContent<EntryDetailModel>(htmlContent, c => c.Content = c.Content.FixLinks())
                            .ToList();

                        searchResultModel.TitleModel = titleModel;

                        searchResultModel.Result = true;

                        return searchResultModel;
                    }

                    searchResultModel.SuggestedTitleModels = _bindingComponent
                        .Binder()
                        .BindModelHtmlContent<SuggestedTitleModel>(htmlContent).ToList();

                    return searchResultModel;
                }
            }
        }

        public async Task<TitleModel> GetTitle(string titleNameIdText, bool? populer = null, int? page = null)
        {
            Uri uri = new Uri($"https://eksisozluk.com/{titleNameIdText}");

            int currentPage = page ?? 1;

            if (populer.HasValue)
            {
                uri = uri.AddParameter("a", "popular");
            }
            uri = uri.AddParameter("p", currentPage.ToString());

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            // TODO : @deniz nested modeller için binding desteklememiz gerekiyor. 
            return await BindHttpRequestMessage(request, htmlContent =>
            {
                TitleModel titleModel = _bindingComponent
                    .Binder()
                    .BindModelHtmlContent<TitleModel>(htmlContent, model => model.TitleNameIdText = titleNameIdText)
                    .FirstOrDefault();

                titleModel = titleModel ??
                             new TitleModel()
                             {
                                 CurrentPage = "1",
                                 PageCount = "1",
                                 TitleNameIdText = titleNameIdText
                             };

                titleModel.EntryDetailModels = _bindingComponent
                    .Binder()
                    .BindModelHtmlContent<EntryDetailModel>(htmlContent, t => t.Content = t.Content.FixLinks()).ToList();

                return titleModel;
            });
        }

        // TODO : @deniz bu yapı Binding yapısına taşınacak.
        private async Task<TModel> BindHttpRequestMessage<TModel>(HttpRequestMessage httpRequestMessage, Func<string, TModel> func)
        {
            Uri uri = httpRequestMessage.RequestUri;

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage))
                {
                    string htmlContent = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new NotFoundException(uri.ToString());
                    }
                    if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new InternalServerErrorException(uri.ToString());
                    }
                    if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new GenericHttpException(httpResponseMessage.StatusCode, uri.ToString());
                    }

                    return func(htmlContent);
                }
            }
        }
    }
}
