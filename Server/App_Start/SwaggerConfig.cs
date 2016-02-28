using System.Web.Http;
using WebActivatorEx;
using Server;
using Swashbuckle.Application;

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
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }
    }
}
