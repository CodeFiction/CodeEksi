using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebActivatorEx;
using Server;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using Swashbuckle.Swagger.Annotations;

namespace Server
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "Code Ekþi");
                        c.OperationFilter<AddResponseFromAttributes>();
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }

    public class AddResponseFromAttributes : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // put the controller ones first
            var attributes =
                apiDescription.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<SwaggerResponseAttribute>()
                .Where(a => a.StatusCode > 0)
                .ToList();

            // then the action attributes so they can replace the controller ones
            var actionAttributes =
                apiDescription.ActionDescriptor.GetCustomAttributes<SwaggerResponseAttribute>()
                .Where(a => a.StatusCode > 0)
                .ToList();

            attributes.AddRange(actionAttributes);

            if (!attributes.Any())
            {
                return;
            }

            IList<string> responseCodes = operation.responses.Select(pair => pair.Key).ToList();

            foreach (string responseCode in responseCodes)
            {
                bool any = attributes.Any(attribute => attribute.StatusCode.ToString() == responseCode);

                if (!any)
                {
                    operation.responses.Remove(responseCode);
                }
            }

            foreach (SwaggerResponseAttribute attr in attributes)
            {
                var httpCode = attr.StatusCode.ToString();
                var description = attr.Description;

                if (description == null)
                {
                    // if we don't have a description, try to get it out of HttpStatusCode
                    HttpStatusCode val;
                    if (Enum.TryParse(attr.StatusCode.ToString(), true, out val))
                    {
                        description = val.ToString();
                    }
                }

                var response = new Response { description = description };
                if (attr.Type != null)
                {
                    response = new Response
                    {
                        description = description,
                        schema = schemaRegistry.GetOrRegister(attr.Type),
                    };
                }

                if (response.schema != null && response.schema.@ref == null)
                {
                    response.schema.@ref = response.schema.type == "string" ? "#/definitions/String" : response.schema.items?.@ref;
                }

                operation.responses.Remove(httpCode);
                operation.responses.Add(httpCode, response);
            }
        }
    }
}
