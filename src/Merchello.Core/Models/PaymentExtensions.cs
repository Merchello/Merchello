using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Services;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Extension methods for <see cref="IPayment"/>
    /// </summary>
    public static class PaymentExtensions
    {
        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment)
        {
            return payment.AppliedPayments(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment, IMerchelloContext merchelloContext)
        {
            return payment.AppliedPayments(merchelloContext.Services.GatewayProviderService);
        }

        /// <summary>
        /// Returns a collection of <see cref="IAppliedPayment"/> for this <see cref="IPayment"/>
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="gatewayProviderService">The <see cref="IGatewayProviderService"/></param>
        /// <returns>A collection of <see cref="IAppliedPayment"/></returns>
        public static IEnumerable<IAppliedPayment> AppliedPayments(this IPayment payment, IGatewayProviderService gatewayProviderService)
        {
            return gatewayProviderService.GetAppliedPaymentsByPaymentKey(payment.Key);
        }

        /// <summary>
        /// Returns a collection of <see cref="IInvoice"/>s this <see cref="IPayment"/> has been applied to
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        public static IEnumerable<IInvoice> AppliedToInvoices(this IPayment payment)
        {
            return payment.AppliedToInvoices(MerchelloContext.Current);
        }

        /// <summary>
        /// Returns a collection of <see cref="IInvoice"/>s this <see cref="IPayment"/> has been applied to
        /// </summary>
        /// <param name="payment">The <see cref="IPayment"/></param>
        /// <param name="merchelloContext">The <see cref="IMerchelloContext"/></param>
        /// <returns>A collection of <see cref="IInvoice"/></returns>
        public static IEnumerable<IInvoice> AppliedToInvoices(this IPayment payment, IMerchelloContext merchelloContext)
        {
            return merchelloContext.Services.InvoiceService.GetInvoicesByPaymentKey(payment.Key);
        }

    }
}