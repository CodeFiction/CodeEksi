namespace Services.Contracts
{
    public interface IBindingComponent
    {
        IModelBinder Bind();
    }
}