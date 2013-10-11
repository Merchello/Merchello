using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// The type of a invoice line item
    /// </summary>
    internal class InvoiceItemTypeField : TypeFieldMapper<InvoiceItemType>, IInvoiceItemTypeField
    {
        internal InvoiceItemTypeField()
        {
            if(CachedTypeFields.IsEmpty) BuildCache();
        }

        #region Overrides TypeFieldMapper<InvoiceItemType>


        internal override sealed void BuildCache()
        {
            AddUpdateCache(InvoiceItemType.Item, new TypeField("Item", "Item", new Guid("576CB1FB-5C0D-45F5-8CCD-94F63D174902")));
            AddUpdateCache(InvoiceItemType.Charge, new TypeField("Charge", "Charge or Fee", new Guid("5574BB84-1C96-4F7E-91FB-CFD7C11162A0")));
            AddUpdateCache(InvoiceItemType.Shipping, new TypeField("Shipping", "Shipping", new Guid("7E69FFD2-394C-44BF-9442-B86F67AEC110")));
            AddUpdateCache(InvoiceItemType.Tax, new TypeField("Tax", "Tax", new Guid("3F4830C8-FB7C-4393-831D-3953525541B3")));
            AddUpdateCache(InvoiceItemType.Credit, new TypeField("Credit", "Credit", new Guid("18DEF584-E92A-42F5-9F6F-A49034DAB34F")));
        }

        /// <summary>
        /// Returns a custom invoice item types or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom invoice item type</param>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Items[alias]);
        }

        #endregion

        /// <summary>
        /// Catalog product sales
        /// </summary>
        public ITypeField Item
        {
            get { return GetTypeField(InvoiceItemType.Item); }
        }

        /// <summary>
        /// A standard charge or fee
        /// </summary>
        public ITypeField Charge
        {
            get { return GetTypeField(InvoiceItemType.Charge); }
        }

        /// <summary>
        /// A shipping specific charge
        /// </summary>
        public ITypeField Shipping
        {
            get { return GetTypeField(InvoiceItemType.Shipping); }
        }

        /// <summary>
        /// A tax related charge
        /// </summary>
        public ITypeField Tax
        {
            get { return GetTypeField(InvoiceItemType.Tax); }
        }

        /// <summary>
        /// A credit
        /// </summary>
        public ITypeField Credit
        {
            get { return GetTypeField(InvoiceItemType.Credit); }
        }


        private static TypeFieldCollection Items
        {
            get { return Fields.InvoiceItem; }
        }


    }
}
