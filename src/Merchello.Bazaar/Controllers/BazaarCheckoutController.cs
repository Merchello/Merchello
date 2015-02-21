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
    /// Controller to render the first page of the checkout.
    /// </summary>
    [PluginController("Bazaar")]
    [RequireSsl("Bazaar:RequireSsl")]
    public class BazaarCheckoutController : CheckoutRenderControllerBase
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
            
            var viewModel = (CheckoutModel)Populate(new CheckoutModel(model.Content));

            var factory = new SalePreparationSummaryFactory(viewModel.Currency, new BasketLineItemFactory(Umbraco, viewModel.CurrentCustomer, viewModel.Currency));

            viewModel.BillingAddress = new AddressFormModel()
                                           {
                                               PostAction = "SaveBillingAddress",
                                               Name = CurrentCustomer.IsAnonymous ? string.Empty : ((ICustomer)CurrentCustomer).FirstName + " " + ((ICustomer)CurrentCustomer).LastName,
                                               Email = CurrentCustomer.IsAnonymous ? string.Empty : ((ICustomer)CurrentCustomer).Email,
                                               Countries = AllCountries
                                                                .Select(x => new SelectListItem()
                                                                {
                                                                    Value = x.CountryCode,
                                                                    Text = x.Name
                                                                }),
                                                Regions = Enumerable.Empty<SelectListItem>(),
                                                ContinuePageId = viewModel.StorePage.Descendant("BazaarCheckoutShipping").Id,
                                                AddressType = AddressType.Billing
                                           };

            viewModel.SaleSummary = factory.Build(Basket.SalePreparation()); 

            return this.View(viewModel.ThemeViewPath("Checkout"), viewModel);
        }
    }
}