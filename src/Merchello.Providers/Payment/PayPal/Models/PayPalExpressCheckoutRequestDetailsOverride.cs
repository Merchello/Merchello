namespace Merchello.Providers.Payment.PayPal.Models
{
    using Merchello.Core.Models;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    /// <summary>
    /// An event model that allows for overriding default PayPal ExpressCheckout settings.
    /// </summary>
    public class PayPalExpressCheckoutRequestDetailsOverride
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressCheckoutRequestDetailsOverride"/> class.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="ecDetails">
        /// The express checkout details.
        /// </param>
        public PayPalExpressCheckoutRequestDetailsOverride(IInvoice invoice, IPayment payment, SetExpressCheckoutRequestDetailsType ecDetails)
        {
            this.Invoice = invoice;
            this.Payment = payment;
            this.ExpressCheckoutDetails = ecDetails;
        }

        /// <summary>
        /// Gets the <see cref="IInvoice"/>.
        /// </summary>
        public IInvoice Invoice { get; private set; }

        /// <summary>
        /// Gets the <see cref="IPayment"/>.
        /// </summary>
        public IPayment Payment { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="SetExpressCheckoutRequestDetailsType"/>.
        /// </summary>
        public SetExpressCheckoutRequestDetailsType ExpressCheckoutDetails { get; set; }
    }
}