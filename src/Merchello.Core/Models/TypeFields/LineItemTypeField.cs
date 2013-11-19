using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    internal sealed class LineItemTypeField : TypeFieldMapper<LineItemType>, ILineItemTypeField
    {
        internal LineItemTypeField()
        {
            if(CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
            AddUpdateCache(LineItemType.Product, new TypeField("Product", "Product", new Guid("D462C051-07F4-45F5-AAD2-D5C844159F04")));
            AddUpdateCache(LineItemType.Shipping, new TypeField("Shipping", "Shipping", new Guid("6F3119EA-53F8-41D0-9249-167B8D32AE81")));
            AddUpdateCache(LineItemType.Tax, new TypeField("Tax", "Tax", new Guid("B73C17BC-50D8-4B67-B343-9F0AF7A6E62E")));
            AddUpdateCache(LineItemType.Discount, new TypeField("Discount", "Discount", new Guid("E7CC502D-DE7C-4C37-8A9C-837760533A76")));
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