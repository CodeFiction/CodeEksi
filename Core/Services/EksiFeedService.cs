using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public async Task<IList<TitleModel>> GetDebeList()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = $"https://eksisozluk.com/debe?_={DateTime.Now.Ticks}";

                using (Stream stream = await httpClient.GetStreamAsync(requestUri))
                {
                    HtmlDocument debeDocument = new HtmlDocument();
                    debeDocument.Load(stream, Encoding.UTF8, true);

                    HtmlNodeCollection titleNodes = debeDocument.DocumentNode.ChildNodes["ol"].ChildNodes;

                    IList<TitleModel> titleModels = new List<TitleModel>();

                    foreach (HtmlNode titleNode in titleNodes)
                    {
                        TitleModel titleModel = new TitleModel();

                        HtmlNode aElement = titleNode.FirstChild;
                        string link = aElement.Attributes["href"].Value;
                        string title = aElement.FirstChild.InnerText;

                        titleModel.Link = link;
                        titleModel.Title = title;

                        titleModels.Add(titleModel);
                    }

                    return titleModels;
                }
            }
        }

        public async Task<IList<TitleModel>> GetPopulerList()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var requestUri = $"https://eksisozluk.com/populer?={DateTime.Now.Ticks}";

                using (Stream stream = await httpClient.GetStreamAsync(requestUri))
                {
                    HtmlDocument debeDocument = new HtmlDocument();
                    debeDocument.Load(stream, Encoding.UTF8, true);

                    HtmlNodeCollection titleNodes = debeDocument.DocumentNode.ChildNodes["ul"].ChildNodes;

                    IList<TitleModel> titleModels = new List<TitleModel>();

                    foreach (HtmlNode titleNode in titleNodes)
                    {
                        TitleModel titleModel = new TitleModel();

                        HtmlNode aElement = titleNode.FirstChild;
                        string link = aElement.Attributes["href"].Value;
                        string title = aElement.InnerText;
                        string entryCountStr = aElement.FirstChild.InnerText;

                        int entryCount;

                        bool parsed = int.TryParse(entryCountStr, out entryCount);

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
