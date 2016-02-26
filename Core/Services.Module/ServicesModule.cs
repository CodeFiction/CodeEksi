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
            builder.RegisterType<EksiFeedService>().As<IEksiFeedService>().InstancePerDependency();
        }

        public override void OnPreLoad()
        {
        }
    }
}
