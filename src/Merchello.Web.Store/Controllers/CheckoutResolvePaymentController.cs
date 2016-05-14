namespace Merchello.Web.Store.Controllers
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;

    using Merchello.Core.Gateways;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Factories;
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Mvc;
    using Merchello.Web.Ui;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A base controller responsible for rendering payments.
    /// </summary>
    [PluginController("Merchello")]
    public class CheckoutResolvePaymentController : CheckoutControllerBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutResolvePaymentController"/> class.
        /// </summary>
        public CheckoutResolvePaymentController()
            : this(new CheckoutContextSettingsFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutResolvePaymentController"/> class.
        /// </summary>
        /// <param name="contextSettingsFactory">
        /// The <see cref="CheckoutContextSettingsFactory"/>.
        /// </param>
        public CheckoutResolvePaymentController(CheckoutContextSettingsFactory contextSettingsFactory)
            : base(contextSettingsFactory)
        {
        }

        #endregion

        /// <summary>
        /// Responsible for rendering the resolved payment form.
        /// </summary>
        /// <param name="view">
        /// The optional view.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <remarks>
        /// This cannot be a child action because of the redirect
        /// </remarks>
        [HttpGet]
        [CheckAjaxRequest]
        public virtual ActionResult PaymentForm(string view = "")
        {
            // Get the previously saved payment method
            // Merchello's PaymentMethod should have been called PaymentMethodSettings but legacy from early version
            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();

            if (paymentMethod == null)
            {
                return this.InvalidCheckoutStagePartial();
            }

            var att = GetGatewayMethodUiAttribute(paymentMethod);

            if (att != null)
            {
                // Use the Merchello to resolve the payment controller and method
                var urlParams =
                PaymentMethodUiControllerResolver.Current.GetUrlActionParamsByGatewayMethodUiAliasOnControllerAndMethod(
                    att.Alias);

                var routeDictionary = new RouteValueDictionary();
                foreach (var tuple in urlParams.RouteParams)
                {
                    routeDictionary.Add(tuple.Item1, tuple.Item2);
                }

                // if the view is set add it to the route values
                if (!view.IsNullOrWhiteSpace()) routeDictionary.Add("view", view);

                try
                {
                    return this.HandlePaymentFormSuccess(urlParams, routeDictionary);
                }
                catch (Exception ex)
                {
                    return this.HandlePaymentFormException(ex);
                }
            }

            var nullRef = new NullReferenceException("Controller for specific PaymentMethod could not be found.  Does the controller have a GatewayMethodUiAttribute attribute for both the controller and render form ActionResult?");
            return this.HandlePaymentFormException(nullRef);
        }

        /// <summary>
        /// Handles payment form success.
        /// </summary>
        /// <param name="urlParams">
        /// The url parameters.
        /// </param>
        /// <param name="routeDictionary">
        /// The <see cref="RouteValueDictionary"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected virtual ActionResult HandlePaymentFormSuccess(UrlActionParams urlParams, RouteValueDictionary routeDictionary)
        {
            return this.RedirectToAction(urlParams.Method, urlParams.Controller, routeDictionary);
        }

        /// <summary>
        /// Handle payment form exception.
        /// </summary>
        /// <param name="ex">
        /// The <see cref="Exception"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// The <see cref="Exception"/> to be handled
        /// </exception>
        protected virtual ActionResult HandlePaymentFormException(Exception ex)
        {
            var logData = MultiLogger.GetBaseLoggingData();
            logData.AddCategory("Controllers");
            MultiLogHelper.Error<CheckoutResolvePaymentController>("Could not render payment form.", ex, logData);
            throw ex;
        }
    }
}