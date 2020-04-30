namespace Merchello.Web.Factories
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A factory responsible for building <see cref="ICheckoutPaymentMethodModel"/>.
    /// </summary>
    /// <typeparam name="TPaymentMethodModel">
    /// The type of <see cref="ICheckoutPaymentMethodModel"/>
    /// </typeparam>
    public class CheckoutPaymentMethodModelFactory<TPaymentMethodModel>
        where TPaymentMethodModel : class, ICheckoutPaymentMethodModel, new()
    {
        /// <summary>
        /// The <see cref="IGatewayContext"/>.
        /// </summary>
        private readonly IGatewayContext _gatewayContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentMethodModelFactory{TPaymentMethodModel}"/> class.
        /// </summary>
        public CheckoutPaymentMethodModelFactory()
           : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentMethodModelFactory{TPaymentMethodModel}"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public CheckoutPaymentMethodModelFactory(IMerchelloContext merchelloContext)
        {
            Ensure.ParameterNotNull(merchelloContext, "merchelloContext");
            _gatewayContext = merchelloContext.Gateways;
        }

        /// <summary>
        /// Creates a <see cref="ICheckoutPaymentMethodModel"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="ICheckoutPaymentMethodModel"/>.
        /// </returns>
        public TPaymentMethodModel Create()
        {
            var gatewayMethods = _gatewayContext.Payment.GetPaymentGatewayMethods().ToArray().OrderBy(x => x.PaymentMethod.Name);
            
            var paymentMethodKey = gatewayMethods.Any() ?
                gatewayMethods.First().PaymentMethod.Key :
                Guid.Empty;

            var paymentMethods = gatewayMethods.Select(x => new SelectListItem
                {
                    Value = x.PaymentMethod.Key.ToString(),
                    Text = x.PaymentMethod.Name
                });

            var model = new TPaymentMethodModel
                {
                    PaymentMethodKey = paymentMethodKey,
                    PaymentMethods = paymentMethods,
                    PaymentGatewayMethods = _gatewayContext.Payment.GetPaymentGatewayMethods()
                };

            return OnCreate(model);
        }

        /// <summary>
        /// Allows for overriding the creation of a <see cref="ICheckoutPaymentMethodModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentMethodModel"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="ICheckoutPaymentMethodModel"/>.
        /// </returns>
        protected virtual TPaymentMethodModel OnCreate(TPaymentMethodModel model)
        {
            return model;
        }
    }
}