namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

    /// <summary>
    /// The campaign offer type field.
    /// </summary>
    internal sealed class CampaignActivityTypeField : TypeFieldMapper<CampaignActivityType>, ICampaignActivityTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CampaignActivityTypeField"/> class.
        /// </summary>
        internal CampaignActivityTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <summary>
        /// Gets the campaign offers.
        /// </summary>
        public static TypeFieldCollection CampaignActivities
        {
            get { return Fields.CampaignActivities; }
        }

        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return CampaignActivities.GetTypeFields().Select(GetTypeField);
            }
        }

        ///// <summary>
        ///// Gets the sale type field.
        ///// </summary>
        //public ITypeField Sale
        //{
        //    get { return GetTypeField(CampaignActivityType.Sale); }
        //}

        /// <summary>
        /// Gets the discount type field.
        /// </summary>
        public ITypeField Discount
        {
            get
            {
                return GetTypeField(CampaignActivityType.Discount);
            }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            //AddUpdateCache(CampaignActivityType.Sale, new TypeField("Sale", "Sale", Constants.TypeFieldKeys.CampaignActivity.SaleKey));
            AddUpdateCache(CampaignActivityType.Discount, new TypeField("Discount", "Discount", Constants.TypeFieldKeys.CampaignActivity.DiscountKey));
            AddUpdateCache(CampaignActivityType.Custom, NotFound);
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
            return GetTypeField(CampaignActivities[alias]);
        }
    }
}