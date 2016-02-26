using System;

namespace Services.Module.Base
{
    public interface IRegisterModuleOrComponent : IRegisterModule
    {
        void Load();

        IRegisterModuleOrComponent Register(Action<ContainerBuilderAdapter> registerAction);
    }

    public interface IRegisterModule
    {
        IRegisterModuleOrComponent RegisterModule<T>() where T : BaseModule, new();
    }
}