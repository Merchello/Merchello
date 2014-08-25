using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class LineItemTypeField : TypeFieldMapper<LineItemType>, ILineItemTypeField
    {
        internal LineItemTypeField()
        {
            if(CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
            AddUpdateCache(LineItemType.Product, new TypeField("Product", "Product", Constants.TypeFieldKeys.LineItem.ProductKey));
            AddUpdateCache(LineItemType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.LineItem.ShippingKey));
            AddUpdateCache(LineItemType.Tax, new TypeField("Tax", "Tax", Constants.TypeFieldKeys.LineItem.TaxKey));
            AddUpdateCache(LineItemType.Discount, new TypeField("Discount", "Discount", Constants.TypeFieldKeys.LineItem.DiscountKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return LineItems.GetTypeFields().Select(GetTypeField);
            }
        }


        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(LineItems[alias]);
        }

        public ITypeField Product {
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

        private static TypeFieldCollection LineItems
        {
            get { return Fields.LineItem; }
        }
    }
}