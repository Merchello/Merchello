namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines a checkout context manager.
    /// </summary>
    public interface ICheckoutContextManagerBase
    {
        /// <summary>
        /// Gets the <see cref="ICheckoutContext"/>.
        /// </summary>
        ICheckoutContext Context { get; }

        /// <summary>
        /// Resets (removes) data.
        /// </summary>
        void Reset();
    }
}