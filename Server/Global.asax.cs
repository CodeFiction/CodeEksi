using System.Web;
using Services.Module;

namespace Server
{
    public class MvcApplication : HttpApplication
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
