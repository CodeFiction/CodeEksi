using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Server.Services;
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
            builder.RegisterType<EskiFeedService>().As<IEksiFeedService>().InstancePerDependency();
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
