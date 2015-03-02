namespace Merchello.Bazaar.Controllers
{
    using System;
    using System.Web.Mvc;

    using Umbraco.Web.Mvc;

    /// <summary>
    /// The account operations controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class AccountOperationsController : SurfaceControllerBase
    {
        /// <summary>
        /// The link to receipt.
        /// </summary>
        /// <param name="receiptContentId">
        /// The id of the receipt page
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpGet]
        public ActionResult LinkToReceipt(int receiptContentId, Guid invoiceKey)
        {
            CustomerContext.SetValue("invoiceKey", invoiceKey.ToString());
            return RedirectToUmbracoPage(receiptContentId);
        }
    }
}