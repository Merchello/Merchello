namespace Merchello.Web.Search
{
    /// <summary>
    /// Represents a base class for CachedQuery.
    /// </summary>
    /// REFACTOR in v3 to be more inline with ContextualCaches
    public abstract class CachedQueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryBase"/> class.
        /// </summary>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers.
        /// </param>
        protected CachedQueryBase(bool enableDataModifiers)
        {
            Initialize(enableDataModifiers);
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable data modifiers.
        /// </summary>
        internal virtual bool EnableDataModifiers { get; set; }

        /// <summary>
        /// Initializes the query.
        /// </summary>
        /// <param name="enableDataModifiers">
        /// A value indicating whether or not to enable data modifiers.
        /// </param>
        private void Initialize(bool enableDataModifiers)
        {
            this.EnableDataModifiers = enableDataModifiers;
        }
    }
}