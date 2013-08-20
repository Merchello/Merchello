using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace Merchello.Core.Models.TypeFields
{
    internal sealed class TypeFieldProvider
    {
        private static readonly ConcurrentDictionary<MerchelloType, ITypeField> CachedTypeFields = new ConcurrentDictionary<MerchelloType, ITypeField>();


        internal static ITypeField GetTypeField(MerchelloType key)
        {
            if(CachedTypeFields.IsEmpty) BuildCache();

            ITypeField typeField;
            return CachedTypeFields.TryGetValue(key, out typeField) ?
                typeField : 
                new TypeField("NotFound", "A TypeField with the configuration specified could not be found", Guid.Empty);           
        }

        internal static MerchelloType GetMerchelloType(Guid key)
        {
            return CachedTypeFields.Keys.FirstOrDefault(x => CachedTypeFields[x].TypeKey == key);
        }

       


        private static void BuildCache()
        {            

            // Key Group
            // Value -> Dictionary<string, ITypeField>

            // Dictionary[TypeFieldGroup.Address][MerchelloType.AddressResidential].Value

            // AddressTypes
            AddUpdateCache(MerchelloType.AddressResidential, new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46")));
            AddUpdateCache(MerchelloType.AddressCommercial, new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360")));

            // BasketTypes
            AddUpdateCache(MerchelloType.BasketBasket, new TypeField("Basket", "Standard Basket", new Guid("C53E3100-2DFD-408A-872E-4380383FAD35")));
            AddUpdateCache(MerchelloType.BasketWishlist, new TypeField("Wishlist", "Wishlist", new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5")));

            // InvoiceItemTypes
            AddUpdateCache(MerchelloType.InvoiceItemProduct, new TypeField("Product", "Product", new Guid("576CB1FB-5C0D-45F5-8CCD-94F63D174902")));
            AddUpdateCache(MerchelloType.InvoiceItemCharge, new TypeField("Charge", "Charge or Fee", new Guid("5574BB84-1C96-4F7E-91FB-CFD7C11162A0")));
            AddUpdateCache(MerchelloType.InvoiceItemShipping, new TypeField("Shipping", "Shipping", new Guid("7E69FFD2-394C-44BF-9442-B86F67AEC110")));
            AddUpdateCache(MerchelloType.InvoiceItemTax, new TypeField("Tax", "Tax", new Guid("3F4830C8-FB7C-4393-831D-3953525541B3")));
            AddUpdateCache(MerchelloType.InvoiceItemCredit, new TypeField("Credit", "Credit", new Guid("18DEF584-E92A-42F5-9F6F-A49034DAB34F")));

            // Payment Method Types
            AddUpdateCache(MerchelloType.PaymentMethodCash, new TypeField("Cash", "Cash", new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5")));
            AddUpdateCache(MerchelloType.PaymentMethodCreditCard, new TypeField("CreditCard", "Credit Card", new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A")));
            AddUpdateCache(MerchelloType.PaymentMethodPurchaseOrder, new TypeField("PurchaseOrder", "Purchase Order", new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E")));

            // Ship Method Types
            AddUpdateCache(MerchelloType.ShipMethodFlatRate, new TypeField("FlatRate", "Flat Rate", new Guid("1D0B73CF-AE9D-4501-83F5-FA0B2FEE1236")));
            AddUpdateCache(MerchelloType.ShipMethodPercentTotal, new TypeField("PercentTotal", "Percent of Total", new Guid("B056DA45-3FB0-49AE-8349-6FCEB1465DF6")));
            AddUpdateCache(MerchelloType.ShipMethodCarrier, new TypeField("Carrier", "Carrier", new Guid("4311536A-9554-43D4-8422-DEAAD214B469")));

        }


        private static void AddUpdateCache(MerchelloType key, ITypeField typeField)
        {
            CachedTypeFields.AddOrUpdate(key, typeField, (x, y) => typeField);
        }
        
    }


}
