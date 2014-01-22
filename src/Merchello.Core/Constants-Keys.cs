using System;

namespace Merchello.Core
{
    public static partial class Constants
    {
        /// <summary>
        /// GatewayProviders
        /// </summary>
        public static class ProviderKeys
        {
            public static class Shipping
            {
                public static Guid RateTableShippingProviderKey = new Guid("AEC7A923-9F64-41D0-B17B-0EF64725F576");
            }
        }

        /// <summary>
        /// TypeFields
        /// </summary>
        public static class TypeFieldKeys
        {
            public static class Address
            {
                public static Guid ResidentialKey = new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46");
                public static Guid CommercialKey = new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360");
            }

            public static class ItemCache
            {
                public static Guid BasketKey = new Guid("C53E3100-2DFD-408A-872E-4380383FAD35");
                public static Guid WishlistKey = new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5");
            }

            public static class LineItem
            {
                public static Guid ProductKey = new Guid("D462C051-07F4-45F5-AAD2-D5C844159F04");
                public static Guid ShippingKey = new Guid("6F3119EA-53F8-41D0-9249-167B8D32AE81");
                public static Guid TaxKey = new Guid("B73C17BC-50D8-4B67-B343-9F0AF7A6E62E");
                public static Guid DiscountKey = new Guid("E7CC502D-DE7C-4C37-8A9C-837760533A76");
            }

            public static class PaymentMethod
            {
                public static Guid CashKey = new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5");
                public static Guid CreditCardKey = new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A");
                public static Guid PurchaseOrderKey = new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E");
            }

            public static class AppliedPayment
            {
                public static Guid CreditRecordKey = new Guid("020F6FF8-1F66-4D90-9FF4-C32A7A5AB32B");
                public static Guid DebitRecordKey = new Guid("916929F0-96FB-430A-886D-F7A83E9A4B9A");
                public static Guid VoidRecordKey = new Guid("F59C7DA6-8252-4891-A5A2-7F6C38766649");
            }

            public static class GatewayProvider
            {
                public static Guid PaymentProviderKey = new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E");
                public static Guid ShippingProviderKey = new Guid("646D3EA7-3B31-45C1-9488-7C0449A564A6");
                public static Guid TaxationProviderKey = new Guid("360B47F9-A4FB-4B96-81B4-A4A497D2B44A");
            }
        }
    }
}