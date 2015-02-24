namespace Merchello.Bazaar.Factories
{
    using System.Collections.Generic;

    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Models;
    using Merchello.Web.Workflow;

    using Umbraco.Core.Models;
    using Umbraco.Web.Models;

    /// <summary>
    /// Defines a ViewModelFactory.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates an <see cref="AccountModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="AccountModel"/>.
        /// </returns>
        AccountModel CreateAccount(RenderModel model);

        /// <summary>
        /// Creates an <see cref="AccountHistoryModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="AccountHistoryModel"/>.
        /// </returns>
        AccountHistoryModel CreateAccountHistory(RenderModel model);

        /// <summary>
        /// Creates a <see cref="BasketModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>.
        /// </param>
        /// <returns>
        /// The <see cref="BasketModel"/>.
        /// </returns>
        BasketModel CreateBasket(RenderModel model, IBasket basket);

        /// <summary>
        /// Creates a <see cref="CheckoutModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>
        /// </param>
        /// <param name="allCountries">
        /// A collection of all countries
        /// </param>
        /// <param name="shipCountries">
        /// Allowable ship countries.
        /// </param>
        /// <returns>
        /// The <see cref="CheckoutModel"/>.
        /// </returns>
        CheckoutModel CreateCheckout(RenderModel model, IBasket basket, IEnumerable<ICountry> allCountries, IEnumerable<ICountry> shipCountries);

        /// <summary>
        /// Creates a <see cref="ProductGroupModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductGroupModel"/>.
        /// </returns>
        ProductGroupModel CreateProductGroup(RenderModel model);

        /// <summary>
        /// Creates a <see cref="ProductModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductModel"/>.
        /// </returns>
        ProductModel CreateProduct(RenderModel model);

        /// <summary>
        /// Creates a <see cref="RegistrationModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="RegistrationModel"/>.
        /// </returns>
        RegistrationModel CreateRegistration(RenderModel model);

        /// <summary>
        /// Creates a <see cref="StoreModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="StoreModel"/>.
        /// </returns>
        StoreModel CreateStore(RenderModel model);

        /// <summary>
        /// Creates a <see cref="WishListModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="WishListModel"/>.
        /// </returns>
        WishListModel CreateWishList(RenderModel model);
    }
}