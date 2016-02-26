using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Services.Module;

namespace Server
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper
                .Create()
                .RegisterModule<ServicesModule>()
                .RegisterModule<ServerModule>()
                .Load();
        }
    }
}
