using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Network;
using AngleSharp.Network.Default;
using AngleSharp.Services.Default;
using HtmlAgilityPack;
using Server.Services.Helpers;
using Services.Contracts;
using Services.Contracts.Models;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Server.Services
{
    public class EksiFeedService : IEksiFeedService
    {
        public async Task<IList<DebeTitleModel>> GetDebeList()
        {
            return await HtmlParser.Parse(
                $"https://eksisozluk.com/debe?_={DateTime.Now.Ticks}", HttpMethod.Get,
                debeDocument => debeDocument.ChildNodes["ol"].SelectNodes("li"),
                titleNodes =>
                {
                    IList<DebeTitleModel> titleModels = new List<DebeTitleModel>();

                    foreach (HtmlNode titleNode in titleNodes)
                    {
                        DebeTitleModel titleModel = new DebeTitleModel();

                        HtmlNode aElement = titleNode.SelectNodes("a")[0];
                        string link = aElement.Attributes["href"].Value;
                        string title = aElement.SelectNodes("span")[0].InnerText;

                        string decodedUrl = WebUtility.UrlDecode(link);
                        string entryId = $"#{decodedUrl.Split('#')[1]}";

                        titleModel.Link = link;
                        titleModel.Title = title;
                        titleModel.EntryId = entryId;

                        titleModels.Add(titleModel);
                    }

                    return titleModels;
                }
                );
        }

        public async Task<IList<PopulerTitleModel>> GetPopulerList()
        {
            return await HtmlParser.Parse(
                $"https://eksisozluk.com/basliklar/populer?={DateTime.Now.Ticks}", HttpMethod.Get,
                debeDocument =>
                {
                    return debeDocument
                        .SelectNodes("ul")
                        .AsQueryable()
                        .First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value == "topic-list partial")
                        .SelectNodes("li");
                },
                titleNodes =>
                {
                    IList<PopulerTitleModel> titleModels = new List<PopulerTitleModel>();

                    foreach (HtmlNode titleNode in titleNodes)
                    {
                        PopulerTitleModel titleModel = new PopulerTitleModel();

                        HtmlNode aElement = titleNode.SelectNodes("a")[0];
                        string link = aElement.Attributes["href"].Value;
                        string title = aElement.InnerText;
                        string entryCountStr = aElement.SelectNodes("small")[0].InnerText;

                        int entryCount;

                        bool parsed = int.TryParse(entryCountStr, out entryCount);

                        title = title.Substring(0, title.Length - entryCountStr.Length);

                        titleModel.Link = link;
                        titleModel.Title = title;
                        titleModel.EntryCount = parsed ? entryCount : 0;

                        titleModels.Add(titleModel);
                    }

                    return titleModels;
                }
                );
        }

        public async Task<EntryDetailModel> GetEntryById(string entryId)
        {
            return await HtmlParser.Parse(
                $"https://eksisozluk.com/entry/{entryId}", HttpMethod.Get,
                content => content.SelectNodes($"//li[@data-id={entryId}]"),
                content =>
                {
                    var c = content.FirstOrDefault();
                    if (c == null) { return new EntryDetailModel(); }
                    return new EntryDetailModel
                    {
                        EntryAuthor = c.GetAttributeValue("data-author", "anynomous"),
                        Content = c.SelectSingleNode("div").InnerHtml,
                        EntryDate = c.SelectSingleNode("footer").SelectSingleNode("div/a").InnerText,
                        EntryId = entryId
                    };
                }

                );
        }
    }

    public class EskiFeedService2 : IEksiFeedService
    {
        public async Task<IList<DebeTitleModel>> GetDebeList()
        {
            // IConfiguration withDefaultLoader = Configuration.Default.WithDefaultLoader();
            string formattableString = $"https://eksisozluk.com/debe?_={DateTime.Now.Ticks}";

            HttpRequester httpRequester = new HttpRequester();
            Request request = new Request();
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Address = Url.Create(formattableString);
            IResponse response = await httpRequester.RequestAsync(request, CancellationToken.None);
            IDocument document = await BrowsingContext.New().OpenAsync(response, CancellationToken.None);

            string selector = "ol.stats.topic-list.partial > li > a";

            IHtmlCollection<IElement> querySelectorAll = document.QuerySelectorAll(selector);

            return null;
        }

        public Task<IList<PopulerTitleModel>> GetPopulerList()
        {
            return null;
        }

        public Task<EntryDetailModel> GetEntryById(string entryId)
        {
            return null;
        }
    }
}
