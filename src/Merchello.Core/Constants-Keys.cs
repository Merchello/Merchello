namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Merchello constant Guids (keys)
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Default GUID keys for warehouse, invoice and order types
        /// </summary>
        public static class DefaultKeys
        {
            /// <summary>
            /// The default warehouse keys
            /// </summary>
            public static class Warehouse
            {
                /// <summary>
                /// Gets the default warehouse key.
                /// </summary>
                public static Guid DefaultWarehouseKey
                {
                    get { return new Guid("268D4007-8853-455A-89F7-A28398843E5F"); }
                }

                /// <summary>
                /// Gets the default warehouse catalog key.
                /// </summary>
                public static Guid DefaultWarehouseCatalogKey
                {
                    get { return new Guid("B25C2B00-578E-49B9-BEA2-BF3712053C63"); }
                }
            }

            /// <summary>
            /// The default invoice status keys.
            /// </summary>
            public static class InvoiceStatus
            {
                /// <summary>
                /// Gets the unpaid invoice status key.
                /// </summary>
                public static Guid Unpaid
                {
                    get { return new Guid("17ADA9AC-C893-4C26-AA26-234ECEB2FA75"); }
                }

                /// <summary>
                /// Gets the paid invoice status key.
                /// </summary>
                public static Guid Paid
                {
                    get { return new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E"); }
                }

                /// <summary>
                /// Gets the partially paid invoice status key.
                /// </summary>
                public static Guid Partial
                {
                    get { return new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C"); }
                }

                /// <summary>
                /// Gets the cancelled invoice status key.
                /// </summary>
                public static Guid Cancelled
                {
                    get { return new Guid("53077EFD-6BF0-460D-9565-0E00567B5176"); }
                }

                /// <summary>
                /// Gets the fraud invoice status key
                /// </summary>
                public static Guid Fraud
                {
                    get { return new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087"); }
                }
            }

            /// <summary>
            /// The order status keys
            /// </summary>
            public static class OrderStatus
            {
                /// <summary>
                /// Gets the not fulfilled (or not shipped) order status key
                /// </summary>
                public static Guid NotFulfilled
                {
                    get { return new Guid("C54D47E6-D1C9-40D5-9BAF-18C6ADFFE9D0"); }
                }

                /// <summary>
                /// Gets the back order "back ordered" order status key
                /// </summary>
                public static Guid BackOrder
                {
                    get { return new Guid("C47D475F-A075-4635-BBB9-4B9C49AA8EBE"); }
                }

                /// <summary>
                /// Gets the fulfilled (or completed) order status key.
                /// </summary>
                public static Guid Fulfilled
                {
                    get { return new Guid("D5369B84-8CCA-4586-8FBA-F3020F5E06EC"); }
                }

                /// <summary>
                /// Gets the cancelled order status key.
                /// </summary>
                public static Guid Cancelled
                {
                    get { return new Guid("77DAF52E-C79C-4E1B-898C-5E977A9A6027"); }
                }
            }

            /// <summary>
            /// The shipment status.
            /// </summary>
            public static class ShipmentStatus
            {
                /// <summary>
                /// Gets the quoted shipment status key
                /// </summary>
                public static Guid Quoted
                {
                    get { return new Guid("6FA425A9-7802-4DA0-BD33-083C100E30F3"); }
                }

                /// <summary>
                /// Gets the packaging status key.
                /// </summary>
                public static Guid Packaging
                {
                    get { return new Guid("7342DCD6-8113-44B6-BFD0-4555B82F9503"); }
                }

                /// <summary>
                /// Gets the shipment ready status key.
                /// </summary>
                public static Guid Ready
                {
                    get { return new Guid("CB24D43F-2774-4E56-85D8-653E49E3F542"); }
                }

                /// <summary>
                /// Gets the shipment shipped status key.
                /// </summary>
                public static Guid Shipped
                {
                    get { return new Guid("B37BE101-CEC9-4608-9330-54E56FA0537A"); }
                }

                /// <summary>
                /// Gets the delivered shipment status key.
                /// </summary>
                public static Guid Delivered
                {
                    get { return new Guid("3A279633-4919-485D-8C3B-479848A053D9"); }
                }
            }
        }

        /// <summary>
        /// Default gateway provider keys
        /// </summary>
        public static class ProviderKeys
        {
            /// <summary>
            /// The shipping gateway providers keys.
            /// </summary>
            public static class Shipping
            {
                /// <summary>
                /// Gets the fixed rate shipping provider key.
                /// </summary>
                public static Guid FixedRateShippingProviderKey
                {
                    get { return new Guid("AEC7A923-9F64-41D0-B17B-0EF64725F576"); }
                }
            }

            /// <summary>
            /// The taxation gateway provider keys.
            /// </summary>
            public static class Taxation
            {
                /// <summary>
                /// Gets the fixed rate taxation provider key.
                /// </summary>
                public static Guid FixedRateTaxationProviderKey
                {
                    get { return new Guid("A4AD4331-C278-4231-8607-925E0839A6CD"); }
                }
            }

            /// <summary>
            /// The payment gateway provider keys.
            /// </summary>
            public static class Payment
            {
                /// <summary>
                /// Gets the cash payment provider key.
                /// </summary>
                public static Guid CashPaymentProviderKey
                {
                    get { return new Guid("B2612C3D-8BF0-411C-8C56-32E7495AE79C"); }
                }
            }

            /// <summary>
            /// The notification.
            /// </summary>
            public static class Notification
            {
                /// <summary>
                /// Gets the smtp notification provider key.
                /// </summary>
                public static Guid SmtpNotificationProviderKey
                {
                    get { return new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");}
                }
            }
        }

        /// <summary>
        /// Store Settings
        /// </summary>
        public static class StoreSettingKeys
        {
            /// <summary>
            /// Gets the currency code settings key.
            /// </summary>
            public static Guid CurrencyCodeKey
            {
                get { return new Guid("7E62B7AB-E633-4CC1-9C3B-C3C54BF10BF6"); }
            }

            /// <summary>
            /// Gets the next order number settings key.
            /// </summary>
            public static Guid NextOrderNumberKey
            {
                get { return new Guid("FFC51FA0-2AFF-4707-876D-79E6FD726022"); }
            }

            /// <summary>
            /// Gets the next invoice number settings key.
            /// </summary>
            public static Guid NextInvoiceNumberKey
            {
                get { return new Guid("10BF357E-2E91-4888-9AE5-5B9D7E897052"); }
            }

            /// <summary>
            /// Gets the next shipment number key.
            /// </summary>
            public static Guid NextShipmentNumberKey
            {
                get { return new Guid("487F1C4E-DDBC-4DCD-9882-A9F7C78892B5"); }
            }

            /// <summary>
            /// Gets the date format settings key.
            /// </summary>
            public static Guid DateFormatKey
            {
                get { return new Guid("4693279F-85DC-4EEF-AADF-D47DB0CDE974"); }
            }

            /// <summary>
            /// Gets the time format settings key.
            /// </summary>
            public static Guid TimeFormatKey
            {
                get { return new Guid("CBE0472F-3F72-439D-9C9D-FC8F840C1A9D"); }
            }

            /// <summary>
            /// Gets the Unit System settings key.
            /// </summary>
            public static Guid UnitSystemKey
            {
                get { return new Guid("03069B06-27CB-494B-B153-9C295AF2DE85"); }
            }

            /// <summary>
            /// Gets the global taxable settings key.
            /// </summary>
            public static Guid GlobalTaxableKey
            {
                get { return new Guid("02F008D4-6003-4E4A-9F82-B0027D6A6208"); }
            }

            /// <summary>
            /// Gets the global track inventory settings key.
            /// </summary>
            public static Guid GlobalTrackInventoryKey
            {
                get { return new Guid("11D2CBE6-1057-423B-A7C4-B0EF6D07D9A0"); }
            }

            /// <summary>
            /// Gets the global shippable settings key.
            /// </summary>
            public static Guid GlobalShippableKey
            {
                get { return new Guid("43116355-FC53-497F-965B-6227B57A38E6"); }
            }

            /// <summary>
            /// Gets the global shipping is taxable key.
            /// </summary>
            public static Guid GlobalShippingIsTaxableKey
            {
                get { return new Guid("E322F6C7-9AD6-4338-ADAA-0C86353D8192");}
            }
        }

        /// <summary>
        /// The default type field keys
        /// </summary>
        /// TODO these are incomplete
        public static class TypeFieldKeys
        {
            /// <summary>
            /// Address related keys
            /// </summary>
            public static class Address
            {
                /// <summary>
                /// Gets the shipping address key.
                /// </summary>
                public static Guid ShippingAddressKey
                {
                    get { return new Guid("D32D7B40-2FF2-453F-9AC5-51CF1A981E46"); }
                }

                /// <summary>
                /// Gets the billing address key.
                /// </summary>
                public static Guid BillingAddressKey
                {
                    get { return new Guid("5C2A8638-EA32-49AD-8167-EDDFB45A7360"); }
                }
            }

            /// <summary>
            /// Entity related keys
            /// </summary>
            public static class Entity
            {
                /// <summary>
                /// Gets the customer type field key that represents the entity
                /// </summary>
                public static Guid CustomerKey
                {
                    get { return new Guid("1607D643-E5E8-4A93-9393-651F83B5F1A9"); }
                }

                /// <summary>
                /// Gets the gateway provider type field key that represents the entity
                /// </summary>
                public static Guid GatewayProviderKey
                {
                    get { return new Guid("05BD6E6E-37A0-4954-AA34-BB73678543B2"); }
                }

                /// <summary>
                /// Gets the invoice type field key that represents the entity
                /// </summary>
                public static Guid InvoiceKey
                {
                    get { return new Guid("454539B9-D753-4C16-8ED5-5EB659E56665"); }
                }

                /// <summary>
                /// Gets the item cache type field key that represents the entity
                /// </summary>
                public static Guid ItemCacheKey
                {
                    get { return new Guid("9FEFEEF2-7ECE-439C-90E2-02D2F95D9E37"); }
                }

                /// <summary>
                /// Gets the order type field key that represents the entity
                /// </summary>
                public static Guid OrderKey
                {
                    get { return new Guid("CC5B1372-2EFA-49D5-B9F7-6EACD1182C5B"); }
                }

                /// <summary>
                /// Gets the payment type field key that represents the entity
                /// </summary>
                public static Guid PaymentKey
                {
                    get { return new Guid("6263D568-DEE1-41BB-8100-2333ECB4CF08"); }
                }

                /// <summary>
                /// Gets the product type field key that represents the entity
                /// </summary>
                public static Guid ProductKey
                {
                    get { return new Guid("9F923716-A022-4089-A110-1E9B4E1F2AD1"); }
                }

                /// <summary>
                /// Gets the shipment type field key that represents the entity
                /// </summary>
                public static Guid ShipmentKey
                {
                    get { return new Guid("5F26DF00-BD45-4E83-8095-D17AD8D7D3CE"); }
                }

                /// <summary>
                /// Gets the warehouse type field key that represents the entity
                /// </summary>
                public static Guid WarehouseKey
                {
                    get { return new Guid("2367D705-4BEA-4B3E-8329-664823D28316"); }
                }

                /// <summary>
                /// Gets the warehouse catalog type field key that represents the entity
                /// </summary>
                public static Guid WarehouseCatalogKey
                {
                    get { return new Guid("ED7D69B2-7434-40F4-88A7-EDDA9FB50995"); }
                }
            }

            /// <summary>
            /// The item cache keys
            /// </summary>
            public static class ItemCache
            {
                /// <summary>
                /// Gets the basket key.
                /// </summary>
                public static Guid BasketKey
                {
                    get { return new Guid("C53E3100-2DFD-408A-872E-4380383FAD35"); }
                }

                /// <summary>
                /// Gets the backoffice key.
                /// </summary>
                public static Guid BackofficeKey
                {
                    get { return new Guid("E36930FE-6D6A-4EB3-AF95-712629913812"); }
                }


                /// <summary>
                /// Gets the wishlist key.
                /// </summary>
                public static Guid WishlistKey
                {
                    get { return new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5"); }
                }

                /// <summary>
                /// Gets the checkout key.
                /// </summary>
                public static Guid CheckoutKey
                {
                    get { return new Guid("25608A4E-F3DB-43DE-B137-A9E55B1412CE"); }
                }
            }

            /// <summary>
            /// The line item keys
            /// </summary>
            public static class LineItem
            {
                /// <summary>
                /// Gets the product line item type key.
                /// </summary>
                public static Guid ProductKey
                {
                    get { return new Guid("D462C051-07F4-45F5-AAD2-D5C844159F04"); }
                }

                /// <summary>
                /// Gets the shipping line item type key.
                /// </summary>
                public static Guid ShippingKey
                {
                    get { return new Guid("6F3119EA-53F8-41D0-9249-167B8D32AE81"); }
                }

                /// <summary>
                /// Gets the tax line item type key.
                /// </summary>
                public static Guid TaxKey
                {
                    get { return new Guid("B73C17BC-50D8-4B67-B343-9F0AF7A6E62E"); }
                }

                /// <summary>
                /// Gets the discount line item type key.
                /// </summary>
                public static Guid DiscountKey
                {
                    get { return new Guid("E7CC502D-DE7C-4C37-8A9C-837760533A76"); }
                }
            }

            /// <summary>
            /// The payment method type keys
            /// </summary>
            public static class PaymentMethod
            {
                /// <summary>
                /// Gets the cash type key.
                /// </summary>
                public static Guid CashKey
                {
                    get { return new Guid("9C9A7E61-D79C-4ECC-B0E0-B2A502F252C5"); }
                }

                /// <summary>
                /// Gets the credit card type key.
                /// </summary>
                public static Guid CreditCardKey
                {
                    get { return new Guid("CB1354FE-B30C-449E-BD5C-CD50BCBD804A"); }
                }

                /// <summary>
                /// Gets the purchase order type key.
                /// </summary>
                public static Guid PurchaseOrderKey
                {
                    get { return new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E"); }
                }
            }

            /// <summary>
            /// The applied payment type field keys
            /// </summary>
            public static class AppliedPayment
            {
                /// <summary>
                /// Gets the credit record type key.
                /// </summary>
                public static Guid CreditRecordKey
                {
                    get { return new Guid("020F6FF8-1F66-4D90-9FF4-C32A7A5AB32B"); }
                }

                /// <summary>
                /// Gets the debit record tyep key.
                /// </summary>
                public static Guid DebitRecordKey
                {
                    get { return new Guid("916929F0-96FB-430A-886D-F7A83E9A4B9A"); }
                }

                /// <summary>
                /// Gets the void record type key.
                /// </summary>
                public static Guid VoidRecordKey
                {
                    get { return new Guid("F59C7DA6-8252-4891-A5A2-7F6C38766649"); }
                }

                /// <summary>
                /// Gets the refund record type key.
                /// </summary>
                public static Guid RefundRecordKey
                {
                    get { return new Guid("4CFCEB9C-F6F4-4FDC-9C52-CDC285F1B90A"); }
                }

                /// <summary>
                /// Gets the denied record type key.
                /// </summary>
                public static Guid DeniedRecordKey
                {
                    get { return new Guid("2AE1F2E2-DF5F-4087-81C1-4E0EE809948F"); }
                }
            }

            /// <summary>
            /// Gateway provider type field keys
            /// </summary>
            public static class GatewayProvider
            {
                /// <summary>
                /// Gets the payment provider key.
                /// </summary>
                public static Guid PaymentProviderKey
                {
                    get { return new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E");}
                }

                /// <summary>
                /// Gets the notification provider key.
                /// </summary>
                public static Guid NotificationProviderKey
                {
                    get { return new Guid("C5F53682-4C49-4538-87B3-035D30EE3347"); }
                }

                /// <summary>
                /// Gets the shipping provider key.
                /// </summary>
                public static Guid ShippingProviderKey
                {
                    get { return new Guid("646D3EA7-3B31-45C1-9488-7C0449A564A6"); }
                }

                /// <summary>
                /// Gets the taxation provider key.
                /// </summary>
                public static Guid TaxationProviderKey
                {
                    get { return new Guid("360B47F9-A4FB-4B96-81B4-A4A497D2B44A"); }
                }
            }
        }

        /// <summary>
        /// The default notification keys.
        /// </summary>
        internal static class NotificationKeys
        {
            /// <summary>
            /// The invoice notification trigger keys.
            /// </summary>
            internal static class InvoiceTriggers
            {
                /// <summary>
                /// The status changed trigger keys
                /// </summary>
                public static class StatusChanged
                {
                    /// <summary>
                    /// Gets the invoice status changed to paid.
                    /// </summary>
                    public static Guid ToPaid
                    {
                        get { return new Guid("645E7E24-3F97-467D-9923-0CC4DD96468C"); }
                    }

                    /// <summary>
                    /// Gets invoice status changed the to partial.
                    /// </summary>
                    public static Guid ToPartial
                    {
                        get { return new Guid("5AF5E106-697E-438E-B768-876666C4E333"); }
                    }

                    /// <summary>
                    /// Gets the to cancelled.
                    /// </summary>
                    public static Guid ToCancelled
                    {
                        get { return new Guid("BB2AC8F1-B813-41BE-BFEE-F5A95CBB541A"); }
                    }
                }
            }

            /// <summary>
            /// The order triggers.
            /// </summary>
            internal static class OrderTriggers
            {
                /// <summary>
                /// The status changed notification triggers
                /// </summary>
                public static class StatusChanged
                {
                    /// <summary>
                    /// Gets the to back order notification trigger key
                    /// </summary>
                    public static Guid ToBackOrder
                    {
                        get { return new Guid("7E2858CB-B41C-456F-9D25-EED36FCD4FBC"); }
                    }

                    /// <summary>
                    /// Gets the status changed to fulfilled notification trigger key
                    /// </summary>
                    public static Guid ToFulfilled
                    {
                        get { return new Guid("39D15CDF-32B6-41CC-95F0-483A676F90C0"); }
                    }

                    /// <summary>
                    /// Gets the to cancelled.
                    /// </summary>
                    public static Guid ToCancelled
                    {
                        get { return new Guid("6B495772-D1E2-4515-955C-1E65C86C23D4"); }
                    }
                }
            }
        }        
    }
}