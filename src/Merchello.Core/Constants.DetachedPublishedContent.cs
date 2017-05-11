namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Constants for detached published content type.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The detached published content type.
        /// </summary>
        public static class DetachedPublishedContentType
        {
            /// <summary>
            /// Gets the default product variant detached published content type key.
            /// </summary>
            public static Guid DefaultProductVariantDetachedPublishedContentTypeKey
            {
                get { return new Guid("1B291A7A-417D-41D1-A044-9051E6F30A15"); }
            }
        }
    }
}
