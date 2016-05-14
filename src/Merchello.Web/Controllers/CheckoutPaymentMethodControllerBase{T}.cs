namespace Merchello.Web.Controllers
{
    using System;
    using System.Web.Mvc;

    using Merchello.Core.Logging;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;

    using Umbraco.Core;

    /// <summary>
    /// A base controller responsible for handling payment method selection operations.
    /// </summary>
    /// <typeparam name="TPaymentMethodModel">
    /// The type of <see cref="ICheckoutPaymentMethodModel"/>
    /// </typeparam>
    public abstract class CheckoutPaymentMethodControllerBase<TPaymentMethodModel> : CheckoutControllerBase
        where TPaymentMethodModel : class, ICheckoutPaymentMethodModel, new()
    {
        #region Constructors 

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentMethodControllerBase{TPaymentMethodModel}"/> class.
        /// </summary>
        protected CheckoutPaymentMethodControllerBase()
            : this(new CheckoutPaymentMethodModelFactory<TPaymentMethodModel>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentMethodControllerBase{TPaymentMethodModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentMethodModelFactory">
        /// The <see cref="CheckoutPaymentMethodModelFactory{TPaymentMethodModel}"/>.
        /// </param>
        protected CheckoutPaymentMethodControllerBase(CheckoutPaymentMethodModelFactory<TPaymentMethodModel> checkoutPaymentMethodModelFactory)
            : this(checkoutPaymentMethodModelFactory, new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutPaymentMethodControllerBase{TPaymentMethodModel}"/> class.
        /// </summary>
        /// <param name="checkoutPaymentMethodModelFactory">
        /// The <see cref="CheckoutPaymentMethodModelFactory{TPaymentMethodModel}"/>.
        /// </param>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        protected CheckoutPaymentMethodControllerBase(
            CheckoutPaymentMethodModelFactory<TPaymentMethodModel> checkoutPaymentMethodModelFactory,
            CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
            Mandate.ParameterNotNull(checkoutPaymentMethodModelFactory, "checkoutPaymentMethodModelFactory");
            this.CheckoutPaymentMethodModelFactory = checkoutPaymentMethodModelFactory;
        }

        #endregion

        /// <summary>
        /// Gets or sets the factory responsible for building <see cref="ICheckoutPaymentMethodModel"/>.
        /// </summary>
        protected CheckoutPaymentMethodModelFactory<TPaymentMethodModel> CheckoutPaymentMethodModelFactory { get; set; }

        /// <summary>
        /// Sets the payment method for checkout.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentMethodModel"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="NullReferenceException">
        /// Throws an null reference exception if the payment method cannot be found
        /// </exception>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetPaymentMethod(TPaymentMethodModel model)
        {
            try
            {
                var gatewayMethod = GatewayContext.Payment.GetPaymentGatewayMethodByKey(model.PaymentMethodKey);
                if (gatewayMethod == null)
                {
                    var nullRef = new NullReferenceException("Payment method was not found");
                    throw nullRef;
                }

                CheckoutManager.Payment.SavePaymentMethod(gatewayMethod.PaymentMethod);

                return HandleSetPaymentMethodSuccess(model);
            }
            catch (Exception ex)
            {
                return HandleSetPaymentMethodException(model, ex);
            }
        }

        /// <summary>
        /// Responsible for rendering the payment method form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ChildActionOnly]
        public virtual ActionResult PaymentMethodForm(string view = "")
        {
            var model = CheckoutPaymentMethodModelFactory.Create();
            return view.IsNullOrWhiteSpace() ? PartialView(model) : PartialView(view, model);
        }

        /// <summary>
        /// Handles a successful set payment operation.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandleSetPaymentMethodSuccess(TPaymentMethodModel model)
        {
            return RedirectToCurrentUmbracoPage();
        }

        /// <summary>
        /// Handles an exception in the set payment method operation.
        /// </summary>
        /// <param name="model">
        /// The <see cref="ICheckoutPaymentMethodModel"/>.
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
        protected virtual ActionResult HandleSetPaymentMethodException(TPaymentMethodModel model, Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            MultiLogHelper.Error<CheckoutPaymentMethodControllerBase<TPaymentMethodModel>>("Failed to set payment method", ex, logData);
            throw ex;
        }

    }
}