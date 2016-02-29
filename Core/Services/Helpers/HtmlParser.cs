using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Server.Services.Helpers
{
    internal static class HtmlParser
    {
        public static async Task<TModel> Parse<TModel>(string url, HttpMethod method, Func<HtmlNode, HtmlNodeCollection> selector, Func<HtmlNodeCollection, TModel> mapNodesToModel)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = url;

                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(requestUri),
                    Method = method,
                };

                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    HtmlDocument document = new HtmlDocument();
                    document.Load(stream, Encoding.UTF8, true);

                    var nodes = selector(document.DocumentNode);
                    //var titleNodes = document.DocumentNode
                    //    .SelectNodes("ul")
                    //    .AsQueryable()
                    //    .First(
                    //        node =>
                    //            node.Attributes.Contains("class") &&
                    //            node.Attributes["class"].Value == "topic-list partial")
                    //    .SelectNodes("li");

                    return mapNodesToModel(nodes);

                    //IList<PopulerTitleModel> titleModels = new List<PopulerTitleModel>();

                    //foreach (HtmlNode titleNode in titleNodes)
                    //{
                    //    PopulerTitleModel titleModel = new PopulerTitleModel();

                    //    HtmlNode aElement = titleNode.SelectNodes("a")[0];
                    //    string link = aElement.Attributes["href"].Value;
                    //    string title = aElement.InnerText;
                    //    string entryCountStr = aElement.SelectNodes("small")[0].InnerText;

                    //    int entryCount;

                    //    bool parsed = int.TryParse(entryCountStr, out entryCount);

                    //    title = title.Substring(0, title.Length - entryCountStr.Length);

                    //    titleModel.Link = link;
                    //    titleModel.Title = title;
                    //    titleModel.EntryCount = parsed ? entryCount : 0;

                    //    titleModels.Add(titleModel);
                    //}

                    //return titleModels;
                }
            }
        }
    }
}
