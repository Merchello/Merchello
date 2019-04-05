namespace Merchello.Web.Store.Factories
{
    using Core.Gateways.Payment;

    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Providers.Models;
    using Merchello.Providers.Payment.Braintree;
    using Merchello.Providers.Payment.Braintree.Provider;
    using Merchello.Providers.Payment.Braintree.Services;
    using Merchello.Web.Factories;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A factory responsible for creating Braintree payment models.
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of <see cref="BraintreePaymentModel"/>
    /// </typeparam>
    public class BraintreePaymentModelFactory<TPaymentModel> : CheckoutPaymentModelFactory<TPaymentModel>
        where TPaymentModel : BraintreePaymentModel, new()
    {
        /// <summary>
        /// The <see cref="IBraintreeApiService"/>.
        /// </summary>
        private readonly IBraintreeApiService _braintreeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BraintreePaymentModelFactory{TPaymentModel}"/> class.
        /// </summary>
        public BraintreePaymentModelFactory()
        {
            //// D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969 is the Guid from the BraintreeProvider Activation Attribute
            //// [GatewayProviderActivation("D143E0F6-98BB-4E0A-8B8C-CE9AD91B0969", "BrainTree Payment Provider", "BrainTree Payment Provider")]
            var provider = (BraintreePaymentGatewayProvider)MerchelloContext.Current.Gateways.Payment.GetProviderByKey(Merchello.Providers.Constants.Braintree.GatewayProviderSettingsKey);

            //// GetBraintreeProviderSettings() is an extension method with the provider
            //// Note: We don't need to make this Lazy since all of the services are internally lazy loaded so the instantiation hit should be minimal
            this._braintreeApiService = new BraintreeApiService(provider.ExtendedData.GetBrainTreeProviderSettings());
        }

        /// <summary>
        /// Overrides the creation of the <see cref="BraintreePaymentModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="BraintreePaymentModel"/>.
        /// </param>
        /// <param name="customer">
        /// The current customer.
        /// </param>
        /// <param name="paymentMethod">
        /// The <see cref="IPaymentMethod"/>.
        /// </param>
        /// <returns>
        /// The modified <see cref="BraintreePaymentModel"/>.
        /// </returns>
        protected override TPaymentModel OnCreate(TPaymentModel model, ICustomerBase customer, IPaymentMethod paymentMethod)
        {
            model.Token = GetBraintreeToken(customer);
            model.RequireJs = true;
            return base.OnCreate(model, customer, paymentMethod);
        }

        /// <summary>
        /// Overrides the creation of the <see cref="BraintreePaymentModel"/>.
        /// </summary>
        /// <param name="model">
        /// The <see cref="BraintreePaymentModel"/>.
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
        /// The modified <see cref="BraintreePaymentModel"/>.
        /// </returns>
        protected override TPaymentModel OnCreate(TPaymentModel model, ICustomerBase customer, IPaymentMethod paymentMethod, IPaymentResult attempt)
        {
            model.Token = GetBraintreeToken(customer);
            model.RequireJs = true;
            return base.OnCreate(model, customer, paymentMethod, attempt);
        }

        /// <summary>
        /// Gets the Braintree server token.
        /// </summary>
        /// <param name="customer">
        /// The current customer.
        /// </param>
        /// <returns>
        /// The Braintree server token
        /// </returns>
        protected string GetBraintreeToken(ICustomerBase customer)
        {
            var token = customer.IsAnonymous ?
                this._braintreeApiService.Customer.GenerateClientRequestToken() :
                this._braintreeApiService.Customer.GenerateClientRequestToken((ICustomer)customer);

            return token;
        }
    }
}