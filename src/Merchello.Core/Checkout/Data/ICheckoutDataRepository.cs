namespace Merchello.Core.Checkout.Data
{
    /// <summary>
    /// Defines a CheckoutPersistedDataDictionary interface.
    /// </summary>
    public interface ICheckoutDataRepository
    {
        void Save(bool raiseEvents = true);

        string Get(string alias);

        T Get<T>(string alias) where T : class, new();
    }
}