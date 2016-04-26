namespace Merchello.Providers.Payment.PayPal.Services
{
    using Merchello.Core.Models;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Merchello.Providers.Payment.PayPal.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a PayPalExpressCheckoutService.
    /// </summary>
    public interface IPayPalExpressCheckoutService : IService
    {
        /// <summary>
        /// Performs the setup for an express checkout.
        /// </summary>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>.
        /// </param>
        /// <param name="payment">
        /// The <see cref="IPayment"/>
        /// </param>
        /// <returns>
        /// The <see cref="ExpressCheckoutResponse"/>.
        /// </returns>
        ExpressCheckoutResponse SetExpressCheckout(IInvoice invoice, IPayment payment);
    }
} 