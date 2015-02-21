namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Factories;
    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;

    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The controller responsible for rendering the Basket Page.
    /// </summary>
    [PluginController("Bazaar")]
    public class BazaarBasketController : RenderControllerBase
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
            var factory = new BasketLineItemFactory(Umbraco, this.CurrentCustomer, this.Currency);

            var viewModel = (BasketModel)this.Populate(new BasketModel(model.Content));
            viewModel.BasketTable = new BasketTableModel
                                        {
                                            Items = this.Basket.Items.Select(factory.Build).ToArray(),
                                            TotalPrice = this.Basket.Items.Sum(x => x.TotalPrice),
                                            Currency = viewModel.Currency,
                                            CheckoutPage = viewModel.StorePage.Descendant("BazaarCheckout"),
                                            ContinueShoppingPage = viewModel.ProductGroups.Any() ? 
                                                (IPublishedContent)viewModel.ProductGroups.First() :
                                                viewModel.StorePage,
                                            ShowWishList = viewModel.ShowWishList,
                                            WishListPageId = viewModel.WishListPage.Id,
                                            BasketPageId = viewModel.BasketPage.Id
                                        };
            
            return View(viewModel.ThemeViewPath("Basket"), viewModel);
        }

    }
}