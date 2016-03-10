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

        public async Task<IList<DebeTitleHeaderModel>> GetDebeList()
        {
            var debeTitleModels = await _bindingComponent
                .Binder()
                .WithUrl("https://eksisozluk.com/debe")
                .WithQueryString(new KeyValuePair<string, string>("_", DateTime.Now.Ticks.ToString()))
                .WithHeader(new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest"))
                .BindModel<DebeTitleHeaderModel>(model =>
                {
                    string decodedUrl = WebUtility.UrlDecode(model.Link);
                    string entryId = $"#{decodedUrl.Split('#')[1]}";

                    model.EntryId = entryId;
                });

            return debeTitleModels.ToList();
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

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
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

                    populerModel = populerModel ?? new PopulerModel() {CurrentPage = "1", PageCount = "2"};

                    populerModel.PopulerTitleHeaderModels = populerTitleHeaderModels;

                    return populerModel;
                }
            }
        }

        public async Task<EntryDetailModel> GetEntryById(string entryId)
        {
            IEnumerable<EntryDetailModel> entryDetailModels = await _bindingComponent
                .Binder()
                .WithUrl($"https://eksisozluk.com/entry/{entryId}")
                .WithCssSelectorParameter(new KeyValuePair<string, string>("entryId", entryId))
                .BindModel<EntryDetailModel>();

            return entryDetailModels.FirstOrDefault();
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
                            .BindModelHtmlContent<EntryDetailModel>(htmlContent).ToList();

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

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
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
                        .BindModelHtmlContent<EntryDetailModel>(htmlContent).ToList();

                    return titleModel;
                }
            }
        }
    }
}
