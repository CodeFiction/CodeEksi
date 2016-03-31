using System.Net;

namespace Services.Contracts.Exceptions
{
    public class InternalServerErrorException : GenericHttpException
    {
        public InternalServerErrorException(string requestUrl)
            : base(HttpStatusCode.InternalServerError, requestUrl)
        {
        }
    }
}