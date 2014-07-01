namespace Merchello.Core
{
    /// <summary>
    /// Merchello Product Editor constants 
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The product editor constants
        /// </summary>
        /// <remarks>
        /// TODO These may need to be removed - depending on JP's decision on Product to Content relation
        /// </remarks>
        internal static class ProductEditor
        {
            /// <summary>
            /// Gets the product key property alias.
            /// </summary>
            public static string ProductKeyPropertyAlias
            {
                get { return "merchelloProduct"; }
            }

            /// <summary>
            /// Gets the tab alias.
            /// </summary>
            public static string TabAlias
            {
                get { return "merchProductEditor"; }
            }

            /// <summary>
            /// Gets the tab label.
            /// </summary>
            public static string TabLabel
            {
                get { return "Merchello Product"; }
            }

            /// <summary>
            /// Gets the tab id.
            /// </summary>
            /// TODO: What ID should we use?
            public static int TabId
            {
                get { return -666; }
            }

            /// <summary>
            /// Gets the property alias.
            /// </summary>
            public static string PropertyAlias
            {
                get { return "merchProductEditor"; }
            }
        }
    }
}