namespace Merchello.Bazaar.Factories
{
    using System.Collections.Generic;
    using System.Globalization;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Workflow;

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
        /// <param name="allCountries">
        /// The all Countries.
        /// </param>
        /// <param name="shipCountries">
        /// The ship Countries.
        /// </param>
        /// <returns>
        /// The <see cref="AccountModel"/>.
        /// </returns>
        AccountModel CreateAccount(RenderModel model, IEnumerable<ICountry> allCountries, IEnumerable<ICountry> shipCountries);

        /// <summary>
        /// Creates an <see cref="AccountHistoryModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="invoices">
        /// The collection of <see cref="InvoiceDisplay"/>
        /// </param>
        /// <returns>
        /// The <see cref="AccountHistoryModel"/>.
        /// </returns>
        AccountHistoryModel CreateAccountHistory(RenderModel model, IEnumerable<InvoiceDisplay> invoices);

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
        /// The create checkout confirmation.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="basket">
        /// The basket.
        /// </param>
        /// <param name="shippingRateQuotes">
        /// The shipping rate quotes.
        /// </param>
        /// <param name="paymentMethods">
        /// The payment methods.
        /// </param>
        /// <param name="paymentMethodUiInfos">
        /// The payment Method UI information
        /// </param>
        /// <returns>
        /// The <see cref="CheckoutModel"/>.
        /// </returns>
        CheckoutConfirmationModel CreateCheckoutConfirmation(RenderModel model, IBasket basket, IEnumerable<IShipmentRateQuote> shippingRateQuotes, IEnumerable<IPaymentGatewayMethod> paymentMethods, IEnumerable<PaymentMethodUiInfo> paymentMethodUiInfos);

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
        /// Creates a <see cref="ProductCollectionModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollectionModel"/>.
        /// </returns>
        ProductCollectionModel CreateProductCollection(RenderModel model);

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
        /// The create receipt.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="invoice">
        /// The <see cref="IInvoice"/>
        /// </param>
        /// <returns>
        /// The <see cref="ReceiptModel"/>.
        /// </returns>
        ReceiptModel CreateReceipt(RenderModel model, IInvoice invoice);

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