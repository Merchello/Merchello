namespace Merchello.Core.Models.TypeFields
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

    /// <summary>
    /// The line item type field.
    /// </summary>
    internal sealed class LineItemTypeField : TypeFieldMapper<LineItemType>, ILineItemTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemTypeField"/> class.
        /// </summary>
        internal LineItemTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }



        public ITypeField Product
        {
            get { return GetTypeField(LineItemType.Product); }
        }

        public ITypeField Shipping
        {
            get { return GetTypeField(LineItemType.Shipping); }
        }

        public ITypeField Tax
        {
            get { return GetTypeField(LineItemType.Tax); }
        }

        public ITypeField Discount
        {
            get { return GetTypeField(LineItemType.Discount); }
        }

        public static TypeFieldCollection LineItems
        {
            get { return Fields.LineItem; }
        }

        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return LineItems.GetTypeFields().Select(GetTypeField);
            }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            AddUpdateCache(LineItemType.Product, new TypeField("Product", "Product", Constants.TypeFieldKeys.LineItem.ProductKey));
            AddUpdateCache(LineItemType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.LineItem.ShippingKey));
            AddUpdateCache(LineItemType.Tax, new TypeField("Tax", "Tax", Constants.TypeFieldKeys.LineItem.TaxKey));
            AddUpdateCache(LineItemType.Discount, new TypeField("Discount", "Discount", Constants.TypeFieldKeys.LineItem.DiscountKey));
            AddUpdateCache(LineItemType.Custom, NotFound);
        }

        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(LineItems[alias]);
        }

        
    }
}