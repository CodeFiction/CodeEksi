using System;
using System.ComponentModel;
using System.Text;
using Services.Contracts;

namespace Server.Services
{
    public class BindingComponent : IBindingComponent
    {
        private readonly Func<IModelBinder> _modelBinderFactory;

        public BindingComponent(Func<IModelBinder> modelBinderFactory)
        {
            _modelBinderFactory = modelBinderFactory;
        }

        public IModelBinder Binder()
        {
            IModelBinder modelBinderFactory = _modelBinderFactory();

            return modelBinderFactory;
        }
    }
}
