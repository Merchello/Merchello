namespace Merchello.Implementation.Factories
{
    using Merchello.Core.Checkout;

    /// <summary>
    /// A factory for creating <see cref="CheckoutContextSettings"/>.
    /// </summary>
    public class CheckoutContextSettingsFactory
    {
        /// <summary>
        /// Creates the <see cref="CheckoutContextSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="CheckoutContextSettings"/>.
        /// </returns>
        public ICheckoutContextSettings Create()
        {
            return OnCreate(new CheckoutContextSettings());
        }


        /// <summary>
        /// An overridable method that allows for modifying the <see cref="ICheckoutContextSettings"/> before
        /// being used to instantiate the <see cref="CheckoutContextManagerBase"/>
        /// </summary>
        /// <param name="settings">
        /// The <see cref="ICheckoutContextSettings"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutContextSettings"/>.
        /// </returns>
        protected virtual ICheckoutContextSettings OnCreate(ICheckoutContextSettings settings)
        {
            return settings;
        }
    }
}