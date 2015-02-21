namespace Merchello.Bazaar.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Merchello.Bazaar.Factories;
    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web;

    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;
    using Umbraco.Web.Mvc;

    /// <summary>
    /// The bazaar wish list controller.
    /// </summary>
    [PluginController("Bazaar")]
    [Authorize]
    public class BazaarWishListController : RenderControllerBase
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
 
            var viewModel = (WishListModel)this.Populate(new WishListModel(model.Content));

            //// this is a protected page - so the customer has to be an ICustomer
            var customer = (ICustomer)viewModel.CurrentCustomer;
            var wishList = customer.WishList();

            viewModel.WishListTable = new WishListTableModel()
                                          {
                                              Items = wishList.Items.Select(factory.Build).ToArray(),
                                              Currency = viewModel.Currency,
                                              WishListPageId = viewModel.WishListPage.Id,
                                              BasketPageId = viewModel.BasketPage.Id,
                                              ContinueShoppingPage = viewModel.ProductGroups.Any() ?
                                                (IPublishedContent)viewModel.ProductGroups.First() :
                                                viewModel.StorePage
                                          };

            return this.View(viewModel.ThemeViewPath("WishList"), viewModel);
        }
    }
}