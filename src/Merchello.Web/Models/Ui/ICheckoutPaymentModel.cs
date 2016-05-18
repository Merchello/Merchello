namespace Merchello.Web.Models.Ui
{
    using System;

    /// <summary>
    /// Defines a checkout payment model used for accepting payments.
    /// </summary>
    public interface ICheckoutPaymentModel : IRequireJs
    {
        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets the view data.
        /// </summary>
        PaymentAttemptViewData ViewData { get; set; }
    }
}