using System;
using Merchello.Core.Models;

namespace Merchello.Core
{
    /// <summary>
    /// Hard coded type field values required by Merchello
    /// </summary>
    public partial class Constants
    {

        internal struct AddressType
        {          

            internal static ITypeField Residential
            {
                get { return new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46")); }
            }

            internal static ITypeField Commercial
            {
                get { return new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360")); }
            }

        }

        internal struct BasketType
        {
            internal static ITypeField Basket
            {
                get { return new TypeField("Basket", "Standard Basket", new Guid("C53E3100-2DFD-408A-872E-4380383FAD35")); }
            }

            internal static ITypeField Wishlist
            {
                get { return new TypeField("Wishlist", "Wishlist", new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5")); }
            }
        }

        internal struct InvoiceItemType
        {
          
            public static ITypeField Product
            {
                get { return new TypeField("Product", "Product", new Guid("576CB1FB-5C0D-45F5-8CCD-94F63D174902")); }
            }

            public static ITypeField Charge
            {
                get { return new TypeField("Charge", "Charge or Fee", new Guid("5574BB84-1C96-4F7E-91FB-CFD7C11162A0")); }
            }

            public static ITypeField Shipping
            {
                get { return new TypeField("Shipping", "Shipping", new Guid("7E69FFD2-394C-44BF-9442-B86F67AEC110")); }
            }

            public static ITypeField Tax
            {
                get { return new TypeField("Tax", "Tax", new Guid("3F4830C8-FB7C-4393-831D-3953525541B3")); }
            }

            public static ITypeField Credit
            {
                get { return new TypeField("Credit", "Credit", new Guid("18DEF584-E92A-42F5-9F6F-A49034DAB34F")); }
            }
        }

        internal struct PaymentMethodType
        {
 
            public static ITypeField Cash
            {
                get { return new TypeField("Cash", "Cash", new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5")); }
            }

            public static ITypeField CreditCard
            {
                get { return new TypeField("CreditCard", "Credit Card", new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A")); }
            }

            public static ITypeField PurchaseOrder
            {
                get { return new TypeField("PurchaseOrder", "Purchase Order", new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E")); }
            }
        }

        internal struct ShipMethodType
        {

            public static ITypeField FlatRate
            {
                get { return new TypeField("FlatRate", "Flat Rate", new Guid("1D0B73CF-AE9D-4501-83F5-FA0B2FEE1236")); }
            }

            public static ITypeField PercentTotal
            {
                get { return new TypeField("PercentTotal", "Percent of Total", new Guid("B056DA45-3FB0-49AE-8349-6FCEB1465DF6")); }
            }

            public static ITypeField Carrier
            {
                get { return new TypeField("Carrier", "Carrier", new Guid("4311536A-9554-43D4-8422-DEAAD214B469")); }
            }

        }
                
    }
}
