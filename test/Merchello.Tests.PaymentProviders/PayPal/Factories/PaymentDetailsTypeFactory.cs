namespace Merchello.Tests.PaymentProviders.PayPal.Factories
{
    using System;
    using System.Collections.Generic;

    using global::PayPal.PayPalAPIInterfaceService.Model;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// The payment details type factory.
    /// </summary>
    public class PaymentDetailsTypeFactory
    {
        public PaymentDetailsType Build(IInvoice invoice)
        {

            var taxTotal = invoice.TotalTax();

            

            throw new NotImplementedException();
        }

        /// <summary>
        /// Builds a list of <see cref="PaymentDetailsItemType"/>.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PaymentDetailsItemType}"/>.
        /// </returns>
        public virtual IEnumerable<PaymentDetailsItemType> BuildPaymentDetailsItemTypes(IEnumerable<ILineItem> items)
        {
            var paymentDetailItems = new List<PaymentDetailsItemType>();

            
            throw new NotImplementedException();
        }
    }
}