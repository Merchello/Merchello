namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Gateways;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Logging;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Mvc;

    using Umbraco.Core;

    /// <summary>
    /// A base controller responsible for handling payment method selection operations..
    /// </summary>
    /// <typeparam name="TPaymentModel">
    /// The type of the <see cref="ICheckoutPaymentModel"/>
    /// </typeparam>
    public abstract class CheckoutPaymentControllerBase<TPaymentModel> : CheckoutControllerBase, IPaymentMethodUiController
        where TPaymentModel : class, ICheckoutPaymentModel, new()
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        protected CheckoutPaymentControllerBase()
            : this(new CheckoutPaymentModelFactory<TPaymentModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="CheckoutPaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        protected CheckoutPaymentControllerBase(CheckoutPaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory)
            : this(checkoutPaymentModelFactory, new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentControllerBase{TPaymentModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentModelFactory">
        /// The <see cref="CheckoutPaymentModelFactory{TPaymentModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutPaymentControllerBase(
            CheckoutPaymentModelFactory<TPaymentModel> checkoutPaymentModelFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutPaymentModelFactory, "checkoutPaymentModelFactory");
            this.CheckoutPaymentModelFactory = checkoutPaymentModelFactory;

            // ensures the sub class has a GatewayMethodUiAttribute
            EnsureAttribute();
        }

        #endregion

        /// <summary>
        /// Gets the checkout payment model factory.
        /// </summary>
        protected CheckoutPaymentModelFactory<TPaymentModel> CheckoutPaymentModelFactory { get; private set; }

        /// <summary>
        /// Handles the successful payment.
        /// </summary>
        /// <param name="result">
        /// The <see cref="IPaymentResult"/>.
        /// </param>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandlePaymentSuccess(IPaymentResult result, TPaymentModel model)
        {
            return CurrentUmbracoPage();
        }

        /// <summary>
        /// Handles a payment exception.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentModel"/>.
        /// </param>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled
        /// </exception>
        protected virtual ActionResult HandlePaymentException(TPaymentModel model, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            MultiLogHelper.Error<CheckoutPaymentControllerBase<TPaymentModel>>("Failed payment operation.", ex, logData);
            throw ex;
        }

        /// <summary>
        /// Ensures the class is decorated with a <see cref="GatewayMethodUiAttribute"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// Throws a null reference exception if the <see cref="GatewayMethodUiAttribute"/> is not found
        /// </exception>
        private void EnsureAttribute()
        {
            // Ensure GatewayMethodUiAttribute
            var att = GetType().GetCustomAttribute<GatewayMethodUiAttribute>(false);
            if (att == null)
            {
                var logData = MultiLogger.GetBaseLoggingData();
                logData.AddCategory("Controllers");
                var nullRef = new NullReferenceException("Implementation of CheckoutPaymentControllerBase must be decorated with a GatewayMethodUiAttribute");
                MultiLogHelper.Error<CheckoutPaymentControllerBase<TPaymentModel>>("GatewayMethodUiAttribute was not found.", nullRef, logData);
                throw nullRef;
            }
        }
    }
}