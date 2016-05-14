namespace Merchello.FastTrack.Controllers
{
    using System.Web.Mvc;

    using Merchello.FastTrack.Models;
    using Merchello.Web.Controllers;
    using Merchello.Web.Store.Models;

    using Umbraco.Core;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// A controller responsible for handling FastTrack payment method selection operations.
    /// </summary>
    [PluginController("FastTrack")]
    public class CheckoutPaymentMethodController : CheckoutPaymentMethodControllerBase<FastTrackPaymentMethodModel>
    {
        /// <summary>
        /// Overrides the successful set payment operation.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        protected override ActionResult HandleSetPaymentMethodSuccess(FastTrackPaymentMethodModel model)
        {
            return model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                Redirect(model.SuccessRedirectUrl) :
                base.HandleSetPaymentMethodSuccess(model);
        }
    }
}