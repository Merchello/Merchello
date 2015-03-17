namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// The CampaignActivityTypeField interface.
    /// </summary>
    public interface ICampaignActivityTypeField : ITypeFieldMapper<CampaignActivityType>
    {
        /// <summary>
        /// Gets the campaign sale.
        /// </summary>
        ITypeField Sale { get; }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        ITypeField Discount { get; }
    }
}