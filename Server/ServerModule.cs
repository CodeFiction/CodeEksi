using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Server.App_Start;
using Services.Module.Base;

namespace Server
{
    public class ServerModule : BaseModule
    {
        public override void OnBuildComplete(IContainer container)
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        public override void OnLoad(ContainerBuilder builder)
        {
            HttpConfiguration config = GlobalConfiguration.Configuration;

            GlobalConfiguration.Configure(WebApiConfig.Register);
            SwaggerConfig.Register();            

            builder.RegisterApiControllers(GetWebEntryAssembly()).InstancePerDependency();
            builder.RegisterWebApiFilterProvider(config);
        }

        public override void OnPreLoad()
        {
        }

        private static Assembly GetWebEntryAssembly()
        {
            if (HttpContext.Current == null || HttpContext.Current.ApplicationInstance == null)
            {
                return null;
            }

            Type type = HttpContext.Current.ApplicationInstance.GetType();
            while (type != null && type.Namespace == "ASP")
            {
                type = type.BaseType;
            }

            return type?.Assembly;
        }
    }
}