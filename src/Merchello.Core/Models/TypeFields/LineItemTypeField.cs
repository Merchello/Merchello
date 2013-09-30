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
        }

        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(LineItems[alias]);
        }

        public ITypeField Product {
            get { return GetTypeField(LineItemType.Product); }
        }

        private static TypeFieldCollection LineItems
        {
            get { return Fields.LineItem; }
        }
    }
}