namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "checkout". .
    /// </summary>
    public interface ICheckoutSection : IMerchelloConfigurationSection
    {
        /// <inheritdoc/>
        ICheckoutContextSection CheckoutContext { get; }
    }
}