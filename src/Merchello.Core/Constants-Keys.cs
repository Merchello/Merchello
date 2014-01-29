using System;

namespace Merchello.Core
{
    public static partial class Constants
    {

        public static class DefaultKeys
        {            
            public static Guid DefaultWarehouseKey = new Guid("268D4007-8853-455A-89F7-A28398843E5F");
            public static Guid DefaultWarehouseCatalogKey = new Guid("B25C2B00-578E-49B9-BEA2-BF3712053C63");

            public static Guid UnpaidInvoiceStatusKey = new Guid("17ADA9AC-C893-4C26-AA26-234ECEB2FA75");
            public static Guid PaidInvoiceStatusKey = new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E");
            public static Guid PartialInvoiceStatusKey = new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C");
            public static Guid CancelledInvoiceStatusKey = new Guid("53077EFD-6BF0-460D-9565-0E00567B5176");
            public static Guid FraudInvoiceStatusKey = new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087");
        }

        /// <summary>
        /// GatewayProviders
        /// </summary>
        public static class ProviderKeys
        {
            public static class Shipping
            {
                public static Guid RateTableShippingProviderKey = new Guid("AEC7A923-9F64-41D0-B17B-0EF64725F576");
            }

            public static class Taxation
            {
                public static Guid CountryTaxRateTaxationProviderKey = new Guid("A4AD4331-C278-4231-8607-925E0839A6CD");
            }
        }

        /// <summary>
        /// Store Settings
        /// </summary>
        public static class StoreSettingKeys
        {
            public static Guid CurrencyCodeSettingKey = new Guid("7E62B7AB-E633-4CC1-9C3B-C3C54BF10BF6");
            public static Guid NextOrderNumberSettingKey = new Guid("FFC51FA0-2AFF-4707-876D-79E6FD726022");
            public static Guid NextInvoiceNumberSettingKey = new Guid("10BF357E-2E91-4888-9AE5-5B9D7E897052");
            public static Guid DateFormatSettingKey = new Guid("4693279F-85DC-4EEF-AADF-D47DB0CDE974");
            public static Guid TimeFormatSettingKey = new Guid("CBE0472F-3F72-439D-9C9D-FC8F840C1A9D");
            public static Guid GlobalTaxableSettingKey = new Guid("02F008D4-6003-4E4A-9F82-B0027D6A6208");
            public static Guid GlobalTrackInventorySettingKey = new Guid("11D2CBE6-1057-423B-A7C4-B0EF6D07D9A0");
            public static Guid GlobalShippableSettingKey = new Guid("43116355-FC53-497F-965B-6227B57A38E6");
        }

        /// <summary>
        /// TypeFields
        /// </summary>
        public static class TypeFieldKeys
        {
            public static class Address
            {
                public static Guid ShippingAddressKey = new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46");
                public static Guid BillingAddressKey = new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360");
            }

            public static class ItemCache
            {
                public static Guid BasketKey = new Guid("C53E3100-2DFD-408A-872E-4380383FAD35");
                public static Guid WishlistKey = new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5");
                public static Guid CheckoutKey = new Guid("25608A4E-F3DB-43DE-B137-A9E55B1412CE");
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