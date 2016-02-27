﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Services.Contracts;
using Services.Contracts.Models;

namespace Server.Services
{
    public class EksiFeedService : IEksiFeedService
    {
        public async Task<IList<DebeTitleModel>> GetDebeList()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = $"https://eksisozluk.com/debe?_={DateTime.Now.Ticks}";
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(requestUri),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    HtmlDocument debeDocument = new HtmlDocument();
                    debeDocument.Load(stream, Encoding.UTF8, true);

                    HtmlNodeCollection titleNodes = debeDocument.DocumentNode.ChildNodes["ol"].SelectNodes("li");

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
            }
        }

        public async Task<IList<PopulerTitleModel>> GetPopulerList()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = $"https://eksisozluk.com/basliklar/populer?={DateTime.Now.Ticks}";

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(requestUri),
                    Method = HttpMethod.Get,
                };

                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    HtmlDocument debeDocument = new HtmlDocument();
                    debeDocument.Load(stream, Encoding.UTF8, true);

                    var titleNodes = debeDocument.DocumentNode
                        .SelectNodes("ul")
                        .AsQueryable()
                        .First(
                            node =>
                                node.Attributes.Contains("class") &&
                                node.Attributes["class"].Value == "topic-list partial")
                        .SelectNodes("li");


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
            }
        }
    }
}
