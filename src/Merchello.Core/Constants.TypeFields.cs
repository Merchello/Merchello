namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Constant type fields.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The default type field keys
        /// </summary>
        /// TODO ensure all core type field keys have constant values
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
            /// The campaign offer key.
            /// </summary>
            public static class CampaignActivity
            {
                /// <summary>
                /// Gets the key representing a discount type.
                /// </summary>
                public static Guid DiscountKey
                {
                    get { return new Guid("05F735B6-C01E-4B61-863D-EFE7DF8136BF"); }
                }
            }

            /// <summary>
            /// Entity related keys
            /// </summary>
            public static class Entity
            {
                /// <summary>
                /// Gets the campaign offer key.
                /// </summary>
                public static Guid CampaignOfferKey
                {
                    get { return new Guid("0B73E574-4050-47A0-B5A0-7F32B73EF56F"); }
                }

                /// <summary>
                /// Gets the customer type field key that represents the entity
                /// </summary>
                public static Guid CustomerKey
                {
                    get { return new Guid("1607D643-E5E8-4A93-9393-651F83B5F1A9"); }
                }

                /// <summary>
                /// Gets the entity collection key.
                /// </summary>
                public static Guid EntityCollectionKey
                {
                    get
                    {
                        return new Guid("A3C60219-2687-4044-A85C-CC7D6FFCA298");
                    }
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
                /// Gets the product option type field key that represents the entity.
                /// </summary>
                public static Guid ProductOptionKey
                {
                    get { return new Guid("AF98E419-8B4A-44C8-AFDE-A7FFB2B9AF7F"); }
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
                /// Gets the back office key.
                /// </summary>
                public static Guid BackofficeKey
                {
                    get { return new Guid("E36930FE-6D6A-4EB3-AF95-712629913812"); }
                }


                /// <summary>
                /// Gets the wish list key.
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

                /// <summary>
                /// Gets the adjustment line item type key.
                /// </summary>
                public static Guid AdjustmentKey
                {
                    get
                    {
                        return new Guid("AFFE34FA-B842-419C-84D0-309FAB7D2346");
                    }
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
                /// Gets the redirect key.
                /// </summary>
                public static Guid RedirectKey
                {
                    get
                    {
                        return new Guid("A9B043F6-2894-487C-8D2B-2AA88C129790");
                    }
                }

                /// <summary>
                /// Gets the purchase order type key.
                /// </summary>
                public static Guid PurchaseOrderKey
                {
                    get { return new Guid("2B588AE0-7B76-430F-9341-270A8C943E7E"); }
                }

                /// <summary>
                /// Gets the customer credit.
                /// </summary>
                public static Guid CustomerCreditKey
                {
                    get { return new Guid("CD0FB122-DE4E-4F1A-939F-EC5859777EAE"); }
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
                /// Gets the debit record type key.
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
                    get { return new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E"); }
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

    }
}