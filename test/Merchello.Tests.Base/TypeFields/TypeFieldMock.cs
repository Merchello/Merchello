using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Tests.Base.TypeFields
{
    public class TypeFieldMock
    {

        public static ITypeField NullTypeField
        {
            get { return new TypeField("NotFound", "A TypeField with the configuration specified could not be found", Guid.Empty); }
        }

        #region Customer AddressType TypeFields

        public static ITypeField AddressTypeResidential
        {
            get { return new TypeField("Residential", "Residential", new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46")); }
        }

        public static ITypeField AddressTypeCommercial
        {
            get { return new TypeField("Commercial", "Commercial", new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360")); }
        }

        #endregion


        #region BasketType TypeFields

        public static ITypeField BasketBasket
        {
            get { return new TypeField("Basket", "Standard Basket", new Guid("C53E3100-2DFD-408A-872E-4380383FAD35")); }
        }

        public static ITypeField BasketWishlist
        {
            get { return new TypeField("Wishlist", "Wishlist", new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5")); }
        }

        #endregion



        #region TransactionType TypeFields


        public static ITypeField TransactionCredit
        {
            get { return new TypeField("Credit", "Credit", new Guid("020F6FF8-1F66-4D90-9FF4-C32A7A5AB32B")); }
        }

        public static ITypeField TransactionDebit
        {
            get { return new TypeField("Debit", "Debit", new Guid("916929F0-96FB-430A-886D-F7A83E9A4B9A")); }
        }


        #endregion

        #region InvoiceItemType TypeFields

        public static ITypeField InvoiceItemProduct
        {
            get { return new TypeField("Product", "Product", new Guid("576CB1FB-5C0D-45F5-8CCD-94F63D174902")); }
        }

        public static ITypeField InvoiceItemCharge
        {
            get { return new TypeField("Charge", "Charge or Fee", new Guid("5574BB84-1C96-4F7E-91FB-CFD7C11162A0")); }
        }

        public static ITypeField InvoiceItemShipping
        {
            get { return new TypeField("Shipping", "Shipping", new Guid("7E69FFD2-394C-44BF-9442-B86F67AEC110")); }
        }

        public static ITypeField InvoiceItemTax
        {
            get { return new TypeField("Tax", "Tax", new Guid("3F4830C8-FB7C-4393-831D-3953525541B3")); }
        }

        public static ITypeField InvoiceItemCredit
        {
            get { return new TypeField("Credit", "Credit", new Guid("18DEF584-E92A-42F5-9F6F-A49034DAB34F")); }
        }

        #endregion

        #region PaymentMethodType TypeFields


        public static ITypeField PaymentMethodCash
        {
            get { return new TypeField("Cash", "Cash", new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5")); }
        }

        public static ITypeField PaymentMethodCreditCard
        {
            get { return new TypeField("CreditCard", "Credit Card", new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A")); }
        }

        public static ITypeField PaymentMethodPurchaseOrder
        {
            get { return new TypeField("PurchaseOrder", "Purchase Order", new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E")); }
        }
       
        #endregion

        #region ShipMethodType TypeFields

        public static ITypeField ShipMethodFlatRate
        {
            get { return new TypeField("FlatRate", "Flat Rate", new Guid("1D0B73CF-AE9D-4501-83F5-FA0B2FEE1236")); }
        }

        public static ITypeField ShipMethodPercentTotal
        {
            get { return new TypeField("PercentTotal", "Percent of Total", new Guid("B056DA45-3FB0-49AE-8349-6FCEB1465DF6")); }
        }

        public static ITypeField ShipMethodCarrier
        {
            get { return new TypeField("Carrier", "Carrier", new Guid("4311536A-9554-43D4-8422-DEAAD214B469")); }
        }

        #endregion


        #region GatewayProvider

        public static ITypeField GatewayProviderShipping
        {
            get { return new TypeField("Shipping", "Shipping", new Guid("646D3EA7-3B31-45C1-9488-7C0449A564A6")); }
        }

        public static ITypeField GatewayProviderPayment
        {
            get { return new TypeField("Payment", "Payment", new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E")); }
        }

        public static ITypeField GatewayProviderTaxation
        {
            get { return new TypeField("Taxation", "Taxation", new Guid("360B47F9-A4FB-4B96-81B4-A4A497D2B44A")); }
        }

        #endregion

    }
}
