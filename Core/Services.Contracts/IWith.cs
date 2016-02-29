using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IWith
    {
        IWith WithUrl(string url);
        IWith WithRandomQueryString();
        IWith WithHeader(params KeyValuePair<string, string>[] headerValues);
        Task<IEnumerable<TModel>> BindModel<TModel>(Action<TModel> postBindAction = null)
            where TModel : class, new();

        IWith WithCssSelectorParameter(params KeyValuePair<string, string>[] cssSelectorParameters);
    }
}