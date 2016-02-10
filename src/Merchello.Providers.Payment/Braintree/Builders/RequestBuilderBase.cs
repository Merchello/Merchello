namespace Merchello.Providers.Payment.Braintree.Builders
{
    using System;

    using global::Braintree;

    using Merchello.Core;
    using Merchello.Plugin.Payments.Braintree;
    using Merchello.Providers.Payment.Braintree.Models;

    /// <summary>
    /// A builder responsible for building a <see cref="TransactionRequest"/>
    /// </summary>
    /// <typeparam name="T">
    /// The type of request to build
    /// </typeparam>
    public abstract class RequestBuilderBase<T> : IBuilder<T>
        where T : class, new()
    {      
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderBase{T}"/> class. 
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="IMerchelloContext"/>.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if the the settings are null
        /// </exception>
        protected RequestBuilderBase(IMerchelloContext merchelloContext, BraintreeProviderSettings settings)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");
            if (settings == null) throw new ArgumentNullException("settings");

            this.MerchelloContext = merchelloContext;
            this.BraintreeGateway = settings.AsBraintreeGateway();

            this.BraintreeRequest = new T();
        }


        /// <summary>
        /// Gets the transaction request.
        /// </summary>
        protected T BraintreeRequest { get; private set; }

        /// <summary>
        /// Gets the braintree gateway.
        /// </summary>
        protected BraintreeGateway BraintreeGateway { get; private set; }

        /// <summary>
        /// Gets the merchello context.
        /// </summary>
        protected IMerchelloContext MerchelloContext { get; private set; }

        /// <summary>
        /// Returns the built <see cref="TransactionRequest"/>
        /// </summary>
        /// <returns>
        /// The <see cref="TransactionRequest"/>.
        /// </returns>
        public abstract T Build();

    }
}