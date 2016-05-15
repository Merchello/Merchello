namespace Merchello.Web.Store.Models
{
    using System;

    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A model for rendering and accepting cash payments.
    /// </summary>
    public class StorePaymentModel : ICheckoutPaymentModel
    {
        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        public Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the payment process requires JavaScript.
        /// </summary>
        public bool RequireJs { get; set; }

        /// <summary>
        /// Gets or sets the view data.
        /// </summary>
        public PaymentAttemptViewData ViewData { get; set; }
    }
}