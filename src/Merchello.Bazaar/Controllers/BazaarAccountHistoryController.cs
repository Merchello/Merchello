namespace Merchello.Bazaar.Controllers
{
    using System.Web.Mvc;

    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web;

    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar account history controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public partial class BazaarAccountHistoryController : BazaarControllerBase
    {
        /// <summary>
        /// The _merchello.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarAccountHistoryController"/> class.
        /// </summary>
        public BazaarAccountHistoryController()
        {
            _merchello = new MerchelloHelper(MerchelloServices);
        }

        /// <summary>
        /// The index <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="model">
        /// The current render model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public override ActionResult Index(RenderModel model)
        {
            var invoices = _merchello.Query.Invoice.GetByCustomerKey(CurrentCustomer.Key);
            //// TODO currency codes need to be determined by invoice as they no longer should be assoicated with the global setting
            var viewModel = ViewModelFactory.CreateAccountHistory(model, invoices);
            
            return this.View(viewModel.ThemeAccountPath("History"), viewModel);
        }
    }
}