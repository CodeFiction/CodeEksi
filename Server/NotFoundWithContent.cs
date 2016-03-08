using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Server
{
    public class NotFoundWithContent<TContent> : IHttpActionResult
    {
        public NotFoundWithContent(HttpRequestMessage request, TContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }


            Content = content;
            Request = request;
        }

        public TContent Content { get; private set; }

        public HttpRequestMessage Request { get; private set; }


        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        public HttpResponseMessage Execute()
        {
            return Request.CreateResponse(HttpStatusCode.NotFound, Content);
        }
    }
}