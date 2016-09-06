namespace Merchello.Core.Configuration.Sections
{
    /// <summary>
    /// Represents a configuration section for configurations related to Merchello "sales".
    /// </summary>
    public interface ISalesSection : IMerchelloConfigurationSection
    {
        /// <summary>
        /// Gets a value indicating whether or not to generate an order disregarding the payment status of the invoice.
        /// </summary>
        bool AlwaysApproveOrderCreation { get; }
    }
}