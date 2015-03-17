namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

    /// <summary>
    /// The campaign offer type field.
    /// </summary>
    public class CampaignOfferTypeField : TypeFieldMapper<CampaignOfferType>, ICampaignOfferTypeField
    {
        /// <summary>
        /// Gets the campaign offers.
        /// </summary>
        public static TypeFieldCollection CampaignOffers
        {
            get { return Fields.CampaignOffers; }
        }

        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return CampaignOffers.GetTypeFields().Select(GetTypeField);
            }
        }

        /// <summary>
        /// Gets the sale type field.
        /// </summary>
        public ITypeField Sale
        {
            get { return GetTypeField(CampaignOfferType.Sale); }
        }

        /// <summary>
        /// Gets the discount type field.
        /// </summary>
        public ITypeField Discount
        {
            get
            {
                return GetTypeField(CampaignOfferType.Discount);
            }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            AddUpdateCache(CampaignOfferType.Sale, new TypeField("Sale", "Sale", Constants.TypeFieldKeys.CampaignOffer.SaleKey));
            AddUpdateCache(CampaignOfferType.Sale, new TypeField("Discount", "Discount", Constants.TypeFieldKeys.CampaignOffer.DiscountKey));
            AddUpdateCache(CampaignOfferType.Custom, NotFound);
        }

        /// <summary>
        /// Gets a custom type field from the configuration file.
        /// </summary>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <returns>
        /// The <see cref="ITypeField"/>.
        /// </returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(CampaignOffers[alias]);
        }
    }
}