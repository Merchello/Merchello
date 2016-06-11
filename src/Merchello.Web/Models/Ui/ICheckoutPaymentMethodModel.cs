namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Merchello.Core.Gateways.Payment;

    /// <summary>
    /// Defines a payment method model.
    /// </summary>
    public interface ICheckoutPaymentMethodModel : IUiModel
    {
        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment methods.
        /// </summary>
        IEnumerable<SelectListItem> PaymentMethods { get; set; }

        /// <summary>
        /// Gets or sets the provider payment gateway methods.
        /// </summary>
        IEnumerable<IPaymentGatewayMethod> PaymentGatewayMethods { get; set; }
    }
}