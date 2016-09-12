namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class LineItemTypeField : ExtendedTypeFieldMapper<LineItemType>, ILineItemTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineItemTypeField"/> class.
        /// </summary>
        internal LineItemTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Product
        {
            get { return GetTypeField(LineItemType.Product); }
        }

        /// <inheritdoc/>
        public ITypeField Shipping
        {
            get { return GetTypeField(LineItemType.Shipping); }
        }

        /// <inheritdoc/>
        public ITypeField Tax
        {
            get { return GetTypeField(LineItemType.Tax); }
        }

        /// <inheritdoc/>
        public ITypeField Discount
        {
            get { return GetTypeField(LineItemType.Discount); }
        }

        /// <inheritdoc/>
        public ITypeField Adjustment
        {
            get
            {
                return GetTypeField(LineItemType.Adjustment);
            }
        }


        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(LineItemType.Product, new TypeField("Product", "Product", Constants.TypeFieldKeys.LineItem.ProductKey));
            AddUpdateCache(LineItemType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.LineItem.ShippingKey));
            AddUpdateCache(LineItemType.Tax, new TypeField("Tax", "Tax", Constants.TypeFieldKeys.LineItem.TaxKey));
            AddUpdateCache(LineItemType.Discount, new TypeField("Discount", "Discount", Constants.TypeFieldKeys.LineItem.DiscountKey));
            AddUpdateCache(LineItemType.Adjustment, new TypeField("Adjustment", "Adjustment", Constants.TypeFieldKeys.LineItem.AdjustmentKey));
            AddUpdateCache(LineItemType.Custom, NotFound);
        }
    }
}