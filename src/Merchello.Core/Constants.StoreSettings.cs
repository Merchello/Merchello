namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Constants store settings keys.
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// Store Settings
        /// </summary>
        public static class StoreSetting
        {
            /// <summary>
            /// The default currency code.
            /// </summary>
            public const string DefaultCurrencyCode = "USD";

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
                get { return new Guid("E322F6C7-9AD6-4338-ADAA-0C86353D8192"); }
            }

            /// <summary>
            /// Gets the migration key.
            /// </summary>
            public static Guid MigrationKey
            {
                get
                {
                    return new Guid("56044D81-5C1E-4073-A2CB-1BE3412E461B");
                }
            }

            /// <summary>
            /// Gets the global taxation application key.
            /// </summary>
            public static Guid GlobalTaxationApplicationKey
            {
                get { return new Guid("B9653D97-A87B-4F78-BE2D-395665FBE361"); }
            }

            /// <summary>
            /// Gets the default extended content culture key.
            /// </summary>
            public static Guid DefaultExtendedContentCulture
            {
                get { return new Guid("292E477B-E39A-4B8B-9865-334EEE850FD7"); }
            }

            /// <summary>
            /// Gets the HasDomainRecord settings key.
            /// </summary>
            public static Guid HasDomainRecordKey
            {
                get
                {
                    return new Guid("84FC6354-5E84-495F-9CB4-1C753D612AF7");
                }
            }
        }
    }
}