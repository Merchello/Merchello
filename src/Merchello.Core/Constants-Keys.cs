using System;

namespace Merchello.Core
{
    public static partial class Constants
    {

        public static class DefaultKeys
        {            
            public static class Warehouse
            {
                public static Guid DefaultWarehouseKey = new Guid("268D4007-8853-455A-89F7-A28398843E5F");
                public static Guid DefaultWarehouseCatalogKey = new Guid("B25C2B00-578E-49B9-BEA2-BF3712053C63");    
            }
            
            public static class InvoiceStatus
            {
                public static Guid Unpaid = new Guid("17ADA9AC-C893-4C26-AA26-234ECEB2FA75");
                public static Guid Paid = new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E");
                public static Guid Partial = new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C");
                public static Guid Cancelled = new Guid("53077EFD-6BF0-460D-9565-0E00567B5176");
                public static Guid Fraud = new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087");    
            }

            public static class OrderStatus
            {
                public static Guid NotFulfilled = new Guid("C54D47E6-D1C9-40D5-9BAF-18C6ADFFE9D0");
                public static Guid BackOrder = new Guid("C47D475F-A075-4635-BBB9-4B9C49AA8EBE");
                public static Guid Fulfilled = new Guid("D5369B84-8CCA-4586-8FBA-F3020F5E06EC");
                public static Guid Cancelled = new Guid("77DAF52E-C79C-4E1B-898C-5E977A9A6027");
            }
        }

        internal static class NotificationKeys
        {
            internal static class InvoiceTriggers
            {
                public static class  StatusChanged 
                {
                    public static Guid ToPaid = new Guid("645E7E24-3F97-467D-9923-0CC4DD96468C");
                    public static Guid ToPartial = new Guid("5AF5E106-697E-438E-B768-876666C4E333");
                    public static Guid ToCancelled = new Guid("BB2AC8F1-B813-41BE-BFEE-F5A95CBB541A");
                }
            }

            internal static class OrderTriggers
            {
                public static class StatusChanged
                {
                    public static Guid ToBackOrder = new Guid("7E2858CB-B41C-456F-9D25-EED36FCD4FBC");
                    public static Guid ToFulfilled = new Guid("39D15CDF-32B6-41CC-95F0-483A676F90C0");
                    public static Guid ToCancelled = new Guid("6B495772-D1E2-4515-955C-1E65C86C23D4");
                }
            }

            internal static class ShipmentTriggers
            {
                public static Guid Created = new Guid("F1413BB5-4A53-4BCE-8FC2-CD1A04ED3D47");
                public static Guid Deleted = new Guid("E726A03F-E9CF-43AC-84D6-8F41454B47A9");
            }

            internal static class AppliedPaymentTriggers
            {

                public static Guid CreatedAsDenied = new Guid("42608501-A33F-4B93-9D47-F613FF34BF3D");
            }

        }
        

        /// <summary>
        /// GatewayProviders
        /// </summary>
        public static class ProviderKeys
        {
            public static class Shipping
            {
                public static Guid FixedRateShippingProviderKey = new Guid("AEC7A923-9F64-41D0-B17B-0EF64725F576");
            }

            public static class Taxation
            {
                public static Guid FixedRateTaxationProviderKey = new Guid("A4AD4331-C278-4231-8607-925E0839A6CD");
            }

            public static class Payment
            {
                public static Guid CashPaymentProviderKey = new Guid("B2612C3D-8BF0-411C-8C56-32E7495AE79C");
            }
        }

        /// <summary>
        /// Store Settings
        /// </summary>
        public static class StoreSettingKeys
        {
            public static Guid CurrencyCodeKey = new Guid("7E62B7AB-E633-4CC1-9C3B-C3C54BF10BF6");
            public static Guid NextOrderNumberKey = new Guid("FFC51FA0-2AFF-4707-876D-79E6FD726022");
            public static Guid NextInvoiceNumberKey = new Guid("10BF357E-2E91-4888-9AE5-5B9D7E897052");
            public static Guid DateFormatKey = new Guid("4693279F-85DC-4EEF-AADF-D47DB0CDE974");
            public static Guid TimeFormatKey = new Guid("CBE0472F-3F72-439D-9C9D-FC8F840C1A9D");
            public static Guid GlobalTaxableKey = new Guid("02F008D4-6003-4E4A-9F82-B0027D6A6208");
            public static Guid GlobalTrackInventoryKey = new Guid("11D2CBE6-1057-423B-A7C4-B0EF6D07D9A0");
            public static Guid GlobalShippableKey = new Guid("43116355-FC53-497F-965B-6227B57A38E6");
            public static Guid GlobalShippingIsTaxableKey = new Guid("E322F6C7-9AD6-4338-ADAA-0C86353D8192");
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

            public static class Entity
            {
                public static Guid CustomerKey = new Guid("1607D643-E5E8-4A93-9393-651F83B5F1A9");
                public static Guid GatewayProviderKey = new Guid("05BD6E6E-37A0-4954-AA34-BB73678543B2");
                public static Guid InvoiceKey = new Guid("454539B9-D753-4C16-8ED5-5EB659E56665");
                public static Guid ItemCacheKey = new Guid("9FEFEEF2-7ECE-439C-90E2-02D2F95D9E37");
                public static Guid OrderKey = new Guid("CC5B1372-2EFA-49D5-B9F7-6EACD1182C5B");
                public static Guid PaymentKey = new Guid("6263D568-DEE1-41BB-8100-2333ECB4CF08");
                public static Guid ProductKey = new Guid("9F923716-A022-4089-A110-1E9B4E1F2AD1");
                public static Guid ShipmentKey = new Guid("5F26DF00-BD45-4E83-8095-D17AD8D7D3CE");
                public static Guid WarehouseKey = new Guid("2367D705-4BEA-4B3E-8329-664823D28316");
                public static Guid WarehouseCatalogKey = new Guid("ED7D69B2-7434-40F4-88A7-EDDA9FB50995");
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
                public static Guid RefundRecordKey = new Guid("4CFCEB9C-F6F4-4FDC-9C52-CDC285F1B90A");
                public static Guid DeniedRecordKey = new Guid("2AE1F2E2-DF5F-4087-81C1-4E0EE809948F");
            }

            public static class GatewayProvider
            {
                public static Guid PaymentProviderKey = new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E");
                public static Guid NotificationProviderKey = new Guid("C5F53682-4C49-4538-87B3-035D30EE3347");
                public static Guid ShippingProviderKey = new Guid("646D3EA7-3B31-45C1-9488-7C0449A564A6");
                public static Guid TaxationProviderKey = new Guid("360B47F9-A4FB-4B96-81B4-A4A497D2B44A");
            }
        }
    }
}