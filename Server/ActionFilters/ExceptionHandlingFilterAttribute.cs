using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Services.Contracts.Exceptions;
using Services.Contracts.Models;

namespace Server.ActionFilters
{
    public class ExceptionHandlingFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Exception exception = actionExecutedContext.Exception;
            var httpException = exception as GenericHttpException;

            var responseCode = HttpStatusCode.InternalServerError;
            if (httpException != null)
            {
                responseCode = httpException.StatusCode;
            }

            ErrorModel errorModel = new ErrorModel() {Message = exception.Message};

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(responseCode, errorModel);
        }
    }
}