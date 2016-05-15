namespace Merchello.Web.Factories
{
    using System.Collections.Generic;

    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Web.Models.Ui;

    /// <summary>
    /// A factory responsible for building typed <see cref="ICheckoutPaymentModel"/> models..
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of the <see cref="ICheckoutPaymentModel"/>
    /// </typeparam>
    public class CheckoutPaymentModelFactory<TPaymentModel>
        where TPaymentModel : class, ICheckoutPaymentModel, new()
    {
        /// <summary>
        /// Creates a <see cref="ICheckoutPaymentModel"/>.
        /// </summary>
        /// <param name="customer">
        /// The current customer.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        public TPaymentModel Create(ICustomerBase customer, IPaymentMethod paymentMethod)
        {
            var model = new TPaymentModel
                {
                    RequireJs = false,
                    PaymentMethodKey = paymentMethod.Key,
                    PaymentMethodName = paymentMethod.Name,
                    ViewData = new PaymentAttemptViewData()
                };

            return OnCreate(model, customer, paymentMethod);
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutPaymentModel"/>.
        /// </summary>
        /// <param name="customer">
        /// The current customer.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <param name="attempt">
        /// The <see cref="IPaymentResult"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        public TPaymentModel Create(ICustomerBase customer, IPaymentMethod paymentMethod, IPaymentResult attempt)
        {
            var model = Create(customer, paymentMethod);

            model.ViewData.Success = attempt.Payment.Success;
            model.ViewData.InvoiceKey = attempt.Invoice.Key;
            if (attempt.Payment.Result != null) model.ViewData.PaymentKey = attempt.Payment.Result.Key;
            model.ViewData.Exception = attempt.Payment.Exception;
            if (attempt.Payment.Exception != null) model.ViewData.Messages = new List<string> { attempt.Payment.Exception.Message };

            return OnCreate(model, customer, paymentMethod, attempt);
        }


        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutPaymentModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        protected virtual TPaymentModel OnCreate(TPaymentModel model, ICustomerBase customer, IPaymentMethod paymentMethod)
        {
            return model;
        }


        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutPaymentModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <param name="customer">
        /// The current customer.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <param name="attempt">
        /// The <see cref="IPaymentResult"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        protected virtual TPaymentModel OnCreate(TPaymentModel model, ICustomerBase customer, IPaymentMethod paymentMethod, IPaymentResult attempt)
        {
            return model;
        }
    }
}