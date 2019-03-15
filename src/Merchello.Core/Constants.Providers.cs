namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Constant provider keys
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Default gateway provider keys
        /// </summary>
        public static class ProviderKeys
        {
            /// <summary>
            /// The entity collection.
            /// </summary>
            public static class EntityCollection
            {
                /// <summary>
                /// Gets the product specification collection key.
                /// </summary>
                public static Guid EntityFilterGroupProviderKey
                {
                    get
                    {
                        return new Guid("5316C16C-E967-460B-916B-78985BB7CED2");
                    }
                }

                /// <summary>
                /// Gets the static product collection provider key.
                /// </summary>
                public static Guid StaticProductCollectionProviderKey
                {
                    get { return new Guid("4700456D-A872-4721-8455-1DDAC19F8C16"); }
                }

                /// <summary>
                /// Gets the static invoice collection provider key.
                /// </summary>
                public static Guid StaticInvoiceCollectionProviderKey
                {
                    get
                    {
                        return new Guid("240023BB-80F0-445C-84D5-29F5892B2FB8");
                    }
                }

                /// <summary>
                /// Gets the static customer collection provider key.
                /// </summary>
                public static Guid StaticCustomerCollectionProviderKey
                {
                    get
                    {
                        return new Guid("A389D41B-C8F1-4289-BD2E-5FFF01DBBDB1");
                    }
                }

                /// <summary>
                /// Gets the static entity collection collection provider.
                /// </summary>
                public static Guid StaticEntityCollectionCollectionProvider
                {
                    get
                    {
                        return new Guid("A8120A01-E9BF-4204-ADDD-D9553F6F24FE");
                    }
                }
            }

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

                /// <summary>
                /// Gets the Bank Transfer payment provider key.
                /// </summary>
                public static Guid BankTransferProviderKey
                {
                    get { return new Guid("B2612C3D-8BF0-411C-8C56-32E7495AE79U"); }
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

        
       
    }
}