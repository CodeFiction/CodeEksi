namespace Services.Contracts
{
    public interface IBindingComponent
    {
        IModelBinder Binder();
    }
}