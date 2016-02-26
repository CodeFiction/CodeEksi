using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Services.Module.Base;

namespace Services.Module
{
    public class Bootstrapper : IRegisterModuleOrComponent
    {
        private readonly IList<BaseModule> _baseModules;
        private readonly ContainerBuilder _containerBuilder;
        private readonly ContainerBuilderAdapter _containerBuilderAdapter;

        private bool _disposed = false;

        private Bootstrapper()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilderAdapter = new ContainerBuilderAdapter(_containerBuilder);

            _baseModules = new List<BaseModule>();
        }

        public static IContainer Container { get; private set; }

        public static IRegisterModuleOrComponent Create()
        {
            return new Bootstrapper();
        }

        public void Load()
        {
            Container = _containerBuilder.Build();

            foreach (BaseModule baseModule in _baseModules)
            {
                baseModule.OnBuildComplete(Container);
            }
        }

        public IRegisterModuleOrComponent Register(Action<ContainerBuilderAdapter> registerAction)
        {
            registerAction(_containerBuilderAdapter);
            return this;
        }

        // TODO : @deniz override module özelliğini ekleyelim
        public IRegisterModuleOrComponent RegisterModule<T>()
                            where T : BaseModule, new()
        {
            Type moduleType = typeof(T);

            RegisterModule(moduleType);

            return this;
        }

        private void RegisterModule(Type moduleType)
        {
            if (!typeof(BaseModule).IsAssignableFrom(moduleType))
            {
                throw new InvalidOperationException("Type is not a module");
            }

            bool moduleExists = _baseModules.Any(baseModule => baseModule.GetType() == moduleType);

            if (moduleExists)
            {
                return;
            }

            var module = (BaseModule)Activator.CreateInstance(moduleType);

            _baseModules.Add(module);

            _containerBuilder.RegisterModule(module);
        }
    }
}
