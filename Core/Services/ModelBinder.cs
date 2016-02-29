using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Network;
using AngleSharp.Network.Default;
using Services.Contracts;
using Services.Contracts.Binding;

namespace Server.Services
{
    public class ModelBinder : IModelBinder
    {
        public string RequestUrl { get; private set; }
        public bool AddRandomQueryString { get; private set; }
        public IReadOnlyDictionary<string, string> HeaderValues { get; private set; }
        public IReadOnlyDictionary<string, string> CssSelectorParameters { get; private set; }

        public ModelBinder()
        {
            // TODO : @deniz null check yapmamak için kötü bir yönrem saat 01:42 . Mazur görelim
            HeaderValues = new Dictionary<string, string>();
            CssSelectorParameters = new Dictionary<string, string>();
        }

        public IWith WithUrl(string url)
        {
            RequestUrl = url;

            return this;
        }

        public IWith WithRandomQueryString()
        {
            AddRandomQueryString = true;

            return this;
        }

        public IWith WithHeader(params KeyValuePair<string, string>[] headerValues)
        {
            HeaderValues = headerValues.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

            return this;
        }

        public IWith WithCssSelectorParameter(params KeyValuePair<string, string>[] cssSelectorParameters)
        {
            CssSelectorParameters = cssSelectorParameters.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

            return this;
        }

        // Bu methodu optimize etmek lazým
        public async Task<IEnumerable<TModel>> BindModel<TModel>(Action<TModel> postBindAction = null)
            where TModel : class, new()
        {
            string random = AddRandomQueryString ? DateTime.Now.Ticks.ToString() : string.Empty;
            string url = $"{RequestUrl}{random}";

            Type modelType = typeof (TModel);

            BindAttribute bindAttribute = modelType.GetCustomAttributes<BindAttribute>(false).FirstOrDefault();

            if (bindAttribute == null)
            {
                // throw Exception
            }

            // TODO : @deniz bunu genel bir yere taþýmak lazým. Birden fazla parametre olan durumlarda olucak
            string cssSelector = bindAttribute.CssSelector;
            if (CssSelectorParameters.Any(pair => cssSelector.Contains($"{{{pair.Key}}}")))
            {
                string key = Regex.Match(cssSelector, @"\{([^}]*)\}").Groups[1].ToString();
                string value = CssSelectorParameters[key];

                cssSelector = cssSelector.Replace($"{{{key}}}", value);
            }

            if (string.IsNullOrEmpty(cssSelector))
            {
                // throw Exception
            }

            HttpRequester httpRequester = new HttpRequester();
            Request request = new Request();

            foreach (KeyValuePair<string, string> keyValuePair in HeaderValues)
            {
                request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }
            request.Address = Url.Create(url);


            IResponse response = await httpRequester.RequestAsync(request, CancellationToken.None);

            // Response'a göre iþlem yapmak lazým

            IDocument document = await BrowsingContext.New().OpenAsync(response, CancellationToken.None);

            IHtmlCollection<IElement> querySelectorAll = document.QuerySelectorAll(cssSelector);

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

                    // Type conversion yapmak gerekebilir.
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