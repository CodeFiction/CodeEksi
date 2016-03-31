using System.Net;

namespace Services.Contracts.Exceptions
{
    public class NotFoundException : GenericHttpException
    {
        public NotFoundException(string requestUrl) 
            : base(HttpStatusCode.NotFound, requestUrl)
        {
        }
    }
}
