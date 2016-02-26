using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace Services.Module.Base
{
    public class ContainerBuilderAdapter
    {
        private readonly ContainerBuilder _builder;

        public ContainerBuilderAdapter(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(
            Func<IComponentContext, IEnumerable<Parameter>, T> @delegate)
        {
            return _builder.Register(@delegate);
        }

        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterDelegate<T>(
            Func<IComponentContext, T> @delegate)
        {
            return _builder.Register(@delegate);
        }

        public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance)
            where T : class
        {
            return _builder.RegisterInstance(instance);
        }

        public IRegistrationBuilder<TImplementer, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TImplementer>()
        {
            return _builder.RegisterType<TImplementer>();
        }

        public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(
            Type implementationType)
        {
            return _builder.RegisterType(implementationType);
        }
    }
}