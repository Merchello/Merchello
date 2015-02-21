namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Attributes;
    using Merchello.Bazaar.Factories;
    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core;
    using Merchello.Core.Models;
    using Merchello.Web;

    using Umbraco.Web;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar checkout shipping controller.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public class BazaarCheckoutShippingController : CheckoutRenderControllerBase
    {
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

            var viewModel = (CheckoutShippingModel)Populate(new CheckoutShippingModel(model.Content));

            var factory = new SalePreparationSummaryFactory(viewModel.Currency, new BasketLineItemFactory(Umbraco, viewModel.CurrentCustomer, viewModel.Currency));

            var preparation = Basket.SalePreparation();
            viewModel.SaleSummary = factory.Build(preparation);

            //if (preparation.GetBillToAddress() != null)
            //{

            //} 
            //else
            //{ 
            //    viewModel.BillingAddress = new AddressFormModel()
            //    {
            //        PostAction = "SaveShippingAddress",
            //        Name = CurrentCustomer.IsAnonymous ? string.Empty : ((ICustomer)CurrentCustomer).FirstName + " " + ((ICustomer)CurrentCustomer).LastName,
            //        Email = CurrentCustomer.IsAnonymous ? string.Empty : ((ICustomer)CurrentCustomer).Email,
            //        Countries = AllCountries
            //                         .Select(x => new SelectListItem()
            //                         {
            //                             Value = x.CountryCode,
            //                             Text = x.Name
            //                         }),
            //        ContinuePageId = viewModel.StorePage.Descendant("BazaarSale").Id,
            //        AddressType = AddressType.Billing
            //    };
            //}
            return this.View(viewModel.ThemeViewPath("Checkout"), viewModel);
        }
    }
}