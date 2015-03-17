namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// The CampaignOfferTypeField interface.
    /// </summary>
    public interface ICampaignOfferTypeField : ITypeFieldMapper<CampaignOfferType>
    {
        /// <summary>
        /// Gets the campaign sale.
        /// </summary>
        ITypeField Sale { get; }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        ITypeField Discount { get;}
    }
}