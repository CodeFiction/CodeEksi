using Autofac;

namespace Services.Module.Base
{
    public abstract class BaseModule : Autofac.Module
    {
        public abstract void OnBuildComplete(IContainer container);

        public abstract void OnLoad(ContainerBuilder builder);

        // Todo : complate logic on Bootstrapper
        public abstract void OnPreLoad();

        protected sealed override void Load(ContainerBuilder builder)
        {
            OnLoad(builder);
            base.Load(builder);
        }
    }
}