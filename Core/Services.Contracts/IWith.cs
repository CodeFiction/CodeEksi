using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IWith
    {
        IWith WithUrl(string url);
        IWith WithHeader(params KeyValuePair<string, string>[] headerValues);
        Task<IEnumerable<TModel>> BindModel<TModel>(Action<TModel> postBindAction = null)
            where TModel : class, new();

        IWith WithCssSelectorParameter(params KeyValuePair<string, string>[] cssSelectorParameters);
        IWith WithQueryString(params KeyValuePair<string, string>[] queryStringParameters);

        IEnumerable<TModel> BindModelWithStream<TModel>(Stream stream, Action<TModel> postBindAction = null)
            where TModel : class, new();

        IEnumerable<TModel> BindModelHtmlContent<TModel>(string htmlContent, Action<TModel> postBindAction = null)
            where TModel : class, new();
    }
}