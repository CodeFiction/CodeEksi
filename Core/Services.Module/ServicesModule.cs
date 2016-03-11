using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Server.Services;
using Server.Services.Aspects;
using Services.Contracts;
using Services.Module.Base;

namespace Services.Module
{
    public class ServicesModule : BaseModule
    {
        public override void OnBuildComplete(IContainer container)
        {

        }

        public override void OnLoad(ContainerBuilder builder)
        {
            builder.RegisterInstance<ObjectCache>(MemoryCache.Default);
            builder.RegisterType<CacheInterceptor>();

            builder.RegisterType<EskiFeedService>()
                .As<IEksiFeedService>()
                .InstancePerDependency()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof (CacheInterceptor));

            builder.RegisterType<BindingComponent>().As<IBindingComponent>().InstancePerDependency();
            builder.RegisterType<ModelBinder>().As<IModelBinder>().InstancePerDependency();

            builder.Register<Func<IModelBinder>>(context =>
            {
                IComponentContext componentContext = context.Resolve<IComponentContext>(); ;

                return () => componentContext.Resolve<IModelBinder>();
            });
        }

        public override void OnPreLoad()
        {
        }
    }
}
