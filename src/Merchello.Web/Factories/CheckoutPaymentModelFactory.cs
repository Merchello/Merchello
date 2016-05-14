namespace Merchello.Web.Factories
{
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
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        public TPaymentModel Create(IPaymentMethod paymentMethod)
        {
            var model = new TPaymentModel
                {
                    PaymentMethodKey = paymentMethod.Key,
                    PaymentMethodName = paymentMethod.Name
                };

            return OnCreate(model, paymentMethod);
        }

        /// <summary>
        /// Allows for overriding the creation of <see cref="ICheckoutPaymentModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutPaymentModel"/>.
        /// </returns>
        protected TPaymentModel OnCreate(TPaymentModel model, IPaymentMethod paymentMethod)
        {
            return model;
        }
    }
}