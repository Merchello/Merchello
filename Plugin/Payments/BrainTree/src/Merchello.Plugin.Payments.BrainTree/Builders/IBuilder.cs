namespace Merchello.Plugin.Payments.Braintree.Builders
{
    public interface IBuilder<T>
    {
        T Build();
    }
}