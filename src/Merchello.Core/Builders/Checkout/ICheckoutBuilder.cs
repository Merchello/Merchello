namespace Merchello.Core.Builders.Checkout
{
    /// <summary>
    /// Defines the Checkout Builder
    /// </summary>
    public interface ICheckoutBuilder : IBuilder
    {
        /// <summary>
        /// Clears the Checkout Object W
        /// </summary>
        void StartOver();
    }
}