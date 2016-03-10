using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Network;
using Server.Services.Helpers;
using Services.Contracts;
using Services.Contracts.Binding;
using Services.Contracts.Exceptions;
using HtmlParser = AngleSharp.Parser.Html.HtmlParser;
using HttpMethod = System.Net.Http.HttpMethod;

namespace Server.Services
{
    public class ModelBinder : IModelBinder
    {
        public string RequestUrl { get; private set; }
        public IReadOnlyDictionary<string, string> HeaderValues { get; private set; }
        public IReadOnlyDictionary<string, string> CssSelectorParameters { get; private set; }
        public IReadOnlyDictionary<string, string> QueryStringParameters { get; private set; }

        public ModelBinder()
        {
            // TODO : @deniz null check yapmamak için kötü bir yönrem saat 01:42 . Mazur görelim
            HeaderValues = new Dictionary<string, string>();
            CssSelectorParameters = new Dictionary<string, string>();
            QueryStringParameters = new Dictionary<string, string>();
        }

        public IWith WithUrl(string url)
        {
            RequestUrl = url;

            return this;
        }

        public IWith WithQueryString(params KeyValuePair<string, string>[] queryStringParameters)
        {
            QueryStringParameters = queryStringParameters.ToDictionary(keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value);

            return this;
        }

        public IWith WithHeader(params KeyValuePair<string, string>[] headerValues)
        {
            HeaderValues = headerValues.ToDictionary(keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value);

            return this;
        }

        public IWith WithCssSelectorParameter(params KeyValuePair<string, string>[] cssSelectorParameters)
        {
            CssSelectorParameters = cssSelectorParameters.ToDictionary(keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value);

            return this;
        }

        // Bu methodu optimize etmek lazým
        public async Task<IEnumerable<TModel>> BindModel<TModel>(Action<TModel> postBindAction = null)
            where TModel : class, new()
        {
            // TODO : Burayý ayrý bir class'a taþýmak lazým

            Uri uri = new Uri(RequestUrl);

            uri = QueryStringParameters.Aggregate(uri,
                (current, queryStringParameter) =>
                    current.AddParameter(queryStringParameter.Key, queryStringParameter.Value));

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Get,
            };

            foreach (KeyValuePair<string, string> keyValuePair in HeaderValues)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            IHtmlDocument htmlDocument;

            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request))
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new NotFoundException(RequestUrl);
                    }
                    if (httpResponseMessage.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        throw new InternalServerErrorException(RequestUrl);
                    }
                    if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
                    {
                        throw new GenericHttpException(httpResponseMessage.StatusCode, RequestUrl);
                    }

                    Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    return BindModelWithStream(stream, postBindAction);
                }
            }
        }

        public IEnumerable<TModel> BindModelHtmlContent<TModel>(string htmlContent, Action<TModel> postBindAction = null)
            where TModel : class, new()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse(htmlContent);

            return BindModelWithHtmlDocument(htmlDocument, postBindAction);
        }

        public IEnumerable<TModel> BindModelWithStream<TModel>(Stream stream, Action<TModel> postBindAction = null)
            where TModel : class, new()
        {
            HtmlParser htmlParser = new HtmlParser();
            IHtmlDocument htmlDocument = htmlParser.Parse(stream);

            return BindModelWithHtmlDocument<TModel>(htmlDocument, postBindAction);
        }

        private IEnumerable<TModel> BindModelWithHtmlDocument<TModel>(IHtmlDocument htmlDocument, Action<TModel> postBindAction = null)
            where TModel : class, new()
        {
            Type modelType = typeof (TModel);

            BindAttribute bindAttribute = modelType.GetCustomAttributes<BindAttribute>(false).FirstOrDefault();

            if (bindAttribute == null)
            {
                throw new BindAttributeNotFoundException($"BindAttribute not found for model {modelType.FullName}");
            }

            // TODO : @deniz bunu genel bir yere taþýmak lazým. Birden fazla parametre olan durumlarda olucak
            string cssSelector = bindAttribute.CssSelector;

            if (string.IsNullOrEmpty(cssSelector))
            {
                throw new CssSelectorNotFoundException($"BindAttribute not found for model {modelType.FullName}");
            }

            if (CssSelectorParameters.Any(pair => cssSelector.Contains($"{{{pair.Key}}}")))
            {
                string key = Regex.Match(cssSelector, @"\{([^}]*)\}").Groups[1].ToString();
                string value = CssSelectorParameters[key];

                cssSelector = cssSelector.Replace($"{{{key}}}", value);
            }

            IHtmlCollection<IElement> querySelectorAll = htmlDocument.QuerySelectorAll(cssSelector);

            List<TModel> models = new List<TModel>();

            foreach (IElement element in querySelectorAll)
            {
                TModel instance = Activator.CreateInstance<TModel>();
                PropertyInfo[] propertyInfos = modelType.GetProperties();

                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    var propertyBindAttribute = propertyInfo.GetCustomAttributes<BindAttribute>(false).FirstOrDefault();

                    if (propertyBindAttribute == null)
                    {
                        continue;
                    }

                    string elementValue;
                    if (propertyBindAttribute.InnerText)
                    {
                        elementValue = string.IsNullOrEmpty(propertyBindAttribute.CssSelector)
                            ? element.TextContent
                            : element.QuerySelector(propertyBindAttribute.CssSelector).TextContent;
                    }
                    else
                    {
                        elementValue = element.Attributes[propertyBindAttribute.AttributeName].Value;
                    }

                    // TODO : @deniz Type conversion yapmak gerekebilir.
                    propertyInfo.SetValue(instance, elementValue);
                }

                postBindAction?.Invoke(instance);

                // TODO : yield return tarzý birþey kullanýlabilir
                models.Add(instance);
            }

            return models;
        }
    }
}