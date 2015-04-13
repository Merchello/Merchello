namespace Merchello.Bazaar.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.UI.WebControls;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.Account;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core;
    using Merchello.Core.Gateways.Shipping;
    using Merchello.Core.Models;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;
    using Merchello.Web.Workflow;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Web;
    using Umbraco.Web.Models;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// Represents a view model factory.
    /// </summary>
    internal class ViewModelFactory : IViewModelFactory
    {
        /// <summary>
        /// The <see cref="ICustomerBase"/>.
        /// </summary>
        private readonly ICustomerBase _currentCustomer;

        /// <summary>
        /// The <see cref="ICurrency"/>.
        /// </summary>
        private readonly ICurrency _currency;

        /// <summary>
        /// The <see cref="BasketLineItemFactory"/>.
        /// </summary>
        private Lazy<BasketLineItemFactory> _basketLineItemFactory;

        /// <summary>
        /// The <see cref="SalePreparationSummaryFactory"/>.
        /// </summary>
        private Lazy<SalePreparationSummaryFactory> _salePreparationSummaryFactory; 

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="umbraco">
        /// The umbraco.
        /// </param>
        /// <param name="currentCustomer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="currency">
        /// The <see cref="ICurrency"/>
        /// </param>
        public ViewModelFactory(UmbracoHelper umbraco, ICustomerBase currentCustomer, ICurrency currency)
        {
            Mandate.ParameterNotNull(currentCustomer, "currentCustomer");
            Mandate.ParameterNotNull(currency, "currency");
            Mandate.ParameterNotNull(umbraco, "umbraco");

            _currentCustomer = currentCustomer;
            _currency = currency;

            this.Initialize(umbraco);
        }

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
        public AccountModel CreateAccount(RenderModel model, IEnumerable<ICountry> allCountries, IEnumerable<ICountry> shipCountries)
        {
            var viewModel = Build<AccountModel>(model);
            var customer = (ICustomer)viewModel.CurrentCustomer;

            viewModel.Profile = new CustomerProfileModel()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    EmailAddress = customer.Email
                };

            viewModel.AccountProfileModel = new AccountProfileModel()
                {
                    Theme = viewModel.Theme,
                    AccountPageId = viewModel.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    EmailAddress = customer.Email,
                    SetPassword = false
                };

            viewModel.CustomerAddressModel = new CustomerAddressModel()
                                                 {
                                                     Theme = viewModel.Theme,
                                                     CustomerKey = viewModel.CurrentCustomer.Key,
                                                     AccountPageId =  viewModel.Id,
                                                     ShipCountries = shipCountries.Select(x => new SelectListItem()
                                                                                                   {
                                                                                                       Value = x.CountryCode,
                                                                                                       Text = x.Name
                                                                                                   }),
                                                     AllCountries = allCountries.Select(x => new SelectListItem()
                                                                                                 {
                                                                                                    Value  = x.CountryCode,
                                                                                                    Text = x.Name
                                                                                                 })
                                                 };

            return viewModel;
        }

        /// <summary>
        /// Creates an <see cref="AccountHistoryModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="invoices">The collection of the <see cref="InvoiceDisplay"/></param>
        /// <returns>
        /// The <see cref="AccountHistoryModel"/>.
        /// </returns>
        public AccountHistoryModel CreateAccountHistory(RenderModel model, IEnumerable<InvoiceDisplay> invoices)
        {
            var viewModel = this.Build<AccountHistoryModel>(model);
            viewModel.Invoices = invoices;
            return viewModel;
        }

        /// <summary>
        /// Creates a <see cref="BasketModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="basket">
        /// The <see cref="IBasket"/>
        /// </param>
        /// <returns>
        /// The <see cref="BasketModel"/>.
        /// </returns>
        public BasketModel CreateBasket(RenderModel model, IBasket basket)
        {
            var viewModel = this.Build<BasketModel>(model);
            viewModel.BasketTable = new BasketTableModel
            {
                Items = basket.Items.Select(_basketLineItemFactory.Value.Build).ToArray(),
                TotalPrice = basket.Items.Sum(x => x.TotalPrice),
                Currency = viewModel.Currency,
                CheckoutPage = viewModel.StorePage.Descendant("BazaarCheckout"),
                ContinueShoppingPage = viewModel.ProductGroups.Any() ?
                    (IPublishedContent)viewModel.ProductGroups.First() :
                    viewModel.StorePage,
                ShowWishList = viewModel.ShowWishList && !_currentCustomer.IsAnonymous,
                WishListPageId = viewModel.WishListPage.Id,
                BasketPageId = viewModel.BasketPage.Id
            };

            return viewModel;
        }

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
        /// The countries.
        /// </param>
        /// <param name="shipCountries">
        /// A collection of allowable ship countries
        /// </param>
        /// <returns>
        /// The <see cref="CheckoutModel"/>.
        /// </returns>
        public CheckoutModel CreateCheckout(RenderModel model, IBasket basket, IEnumerable<ICountry> allCountries, IEnumerable<ICountry> shipCountries)
        {
            var viewModel = this.Build<CheckoutModel>(model);

            var fullName = string.Empty;
            var emailAddress = string.Empty;
            var shippingAddresses = new List<SelectListItem>();
            var billingAddresses = new List<SelectListItem>();
            if (!_currentCustomer.IsAnonymous)
            {
                var customer = (ICustomer)_currentCustomer;
                fullName = customer.FullName;
                emailAddress = customer.Email;
                if (customer.Addresses.Any())
                {
                    // note: we have to add the initial values to these collections so validation works correctly
                    // since Guids are not nullable types
                    shippingAddresses.AddRange(customer.Addresses.Where(x => x.AddressType == AddressType.Shipping).Select(shipAdr => new SelectListItem() { Value = shipAdr.Key.ToString(), Text = shipAdr.Label }));
                    if (shippingAddresses.Any())
                    {
                        shippingAddresses.Insert(
                            0,
                            new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Enter a new address" });
                    }

                    billingAddresses.AddRange(customer.Addresses.Where(x => x.AddressType == AddressType.Billing).Select(shipAdr => new SelectListItem() { Value = shipAdr.Key.ToString(), Text = shipAdr.Label }));
                    if (billingAddresses.Any())
                    {
                        billingAddresses.Insert(
                            0,
                            new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Enter a new address" });
                    }
                }
            }

            viewModel.SaleSummary = _salePreparationSummaryFactory.Value.Build(basket.SalePreparation());
            viewModel.CheckoutAddressForm = new CheckoutAddressForm()
                {
                    IsAnonymous = viewModel.CurrentCustomer.IsAnonymous,
                    BillingAddresses = billingAddresses,
                    BillingName = fullName,
                    BillingEmail = emailAddress,
                    BillingCountries = allCountries.Select(x => new SelectListItem()
                                                            {
                                                                Value = x.CountryCode,
                                                                Text = x.Name
                                                            }),                                                                  
                    BillingIsShipping = true,
                    ShippingAddresses = shippingAddresses,
                    ShippingName = fullName,
                    ShippingEmail = emailAddress,
                    ShippingCountries = shipCountries.Select(x => new SelectListItem()
                                                                {
                                                                Value = x.CountryCode,
                                                                Text = x.Name
                                                                }),
                    ConfirmSalePageId = viewModel.ContinueCheckoutPage.Id,
                    ThemeName = viewModel.Theme,
                    SaleSummary = viewModel.SaleSummary
                };
           
            return viewModel;
        }

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
        /// The payment Method UI information.
        /// </param>
        /// <returns>
        /// The <see cref="CheckoutModel"/>.
        /// </returns>
        public CheckoutConfirmationModel CreateCheckoutConfirmation(RenderModel model, IBasket basket, IEnumerable<IShipmentRateQuote> shippingRateQuotes, IEnumerable<IPaymentMethod> paymentMethods, IEnumerable<PaymentMethodUiInfo> paymentMethodUiInfos)
        {
            var viewModel = this.Build<CheckoutConfirmationModel>(model);

            var isAnonymous = basket.Customer.IsAnonymous;
            var allowedMethods = new List<IPaymentMethod>();

            foreach (var method in paymentMethods.ToArray())
            {
                // TODO constrain methods to only those allowed by known customers.
            }

            viewModel.CheckoutConfirmationForm = new CheckoutConfirmationForm()
            {
                ThemeName = viewModel.Theme,
                CustomerToken = basket.Customer.Key.ToString().EncryptWithMachineKey(),
                SaleSummary = _salePreparationSummaryFactory.Value.Build(basket.SalePreparation()),
                ShippingQuotes = shippingRateQuotes.Select(x => new SelectListItem()
                                                                    {
                                                                        Value = x.ShipMethod.Key.ToString(),
                                                                        Text = string.Format("{0} ({1})", x.ShipMethod.Name, ModelExtensions.FormatPrice(x.Rate, _currency.Symbol))
                                                                    }),
                PaymentMethods = paymentMethods.Select(x => new SelectListItem()
                                                                {
                                                                    Value = x.Key.ToString(),
                                                                    Text = x.Name
                                                                }),
                PaymentMethodUiInfo = paymentMethodUiInfos,
                ReceiptPageId = viewModel.ReceiptPage.Id
            };

            return viewModel;
        }

        /// <summary>
        /// Creates a <see cref="ProductGroupModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductGroupModel"/>.
        /// </returns>
        public ProductGroupModel CreateProductGroup(RenderModel model)
        {
            return this.Build<ProductGroupModel>(model);
        }

        /// <summary>
        /// Creates a <see cref="ProductModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductModel"/>.
        /// </returns>
        public ProductModel CreateProduct(RenderModel model)
        {
            return this.Build<ProductModel>(model);
        }

        /// <summary>
        /// The create receipt.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <returns>
        /// The <see cref="ReceiptModel"/>.
        /// </returns>
        public ReceiptModel CreateReceipt(RenderModel model, IInvoice invoice)
        {
            var viewModel = this.Build<ReceiptModel>(model);
            viewModel.Invoice = invoice;
            var shippingLineItem = invoice.ShippingLineItems().FirstOrDefault();
            if (shippingLineItem != null)
            {
                var shipment = shippingLineItem.ExtendedData.GetShipment<InvoiceLineItem>();
                viewModel.ShippingAddress = shipment.GetDestinationAddress();
            }
            viewModel.BillingAddress = invoice.GetBillingAddress();
            return viewModel;
        }

        /// <summary>
        /// Creates a <see cref="RegistrationModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="RegistrationModel"/>.
        /// </returns>
        public RegistrationModel CreateRegistration(RenderModel model)
        {
            var viewModel = this.Build<RegistrationModel>(model);
            viewModel.RegistrationLogin = new CombinedRegisterLoginModel()
            {
                Login = new CustomerLoginModel(),
                Registration = new CustomerRegistrationModel
                {
                    MemberTypeName = viewModel.CustomerMemberTypeName.EncryptWithMachineKey(),
                },
                AccountPageId = viewModel.AccountPage.Id
            };

            return viewModel;
        }

        /// <summary>
        /// Creates a <see cref="StoreModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="StoreModel"/>.
        /// </returns>
        public StoreModel CreateStore(RenderModel model)
        {
            return this.Build<StoreModel>(model);
        }

        /// <summary>
        /// Creates a <see cref="WishListModel"/>.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="WishListModel"/>.
        /// </returns>
        public WishListModel CreateWishList(RenderModel model)
        {
            if (_currentCustomer.IsAnonymous)
            {
                var badOperation = new InvalidOperationException("Wish lists cannot be created for anonymous customers");
                LogHelper.Error<ViewModelFactory>("Attempted to create a wish list for an anonymous customer", badOperation);
                throw badOperation;
            }

            var viewModel = this.Build<WishListModel>(model);

            //// this is a protected page - so the customer has to be an ICustomer
            var customer = (ICustomer)viewModel.CurrentCustomer;
            var wishList = customer.WishList();

            viewModel.WishListTable = new WishListTableModel()
            {
                Items = wishList.Items.Select(_basketLineItemFactory.Value.Build).ToArray(),
                Currency = viewModel.Currency,
                WishListPageId = viewModel.WishListPage.Id,
                BasketPageId = viewModel.BasketPage.Id,
                ContinueShoppingPage = viewModel.ProductGroups.Any() ?
                  (IPublishedContent)viewModel.ProductGroups.First() :
                  viewModel.StorePage
            };

            return viewModel;
        }

        /// <summary>
        /// Responsible for building the base view model
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <typeparam name="T">
        /// The type of the view model to build
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        private T Build<T>(RenderModel model) where T : MasterModel
        {
            var type = typeof(T);
            
            var ctrArgs = new[] { typeof(IPublishedContent) };
            var ctrValues = new object[] { model.Content };

            var constructor = type.GetConstructor(ctrArgs);
            if (constructor == null) return default(T);
            var master = (T)constructor.Invoke(ctrValues);

            master.CurrentCustomer = _currentCustomer;
            master.Currency = _currency;
            return master;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="umbraco">
        /// The umbraco.
        /// </param>
        private void Initialize(UmbracoHelper umbraco)
        {
            _basketLineItemFactory = new Lazy<BasketLineItemFactory>(() => new BasketLineItemFactory(umbraco, _currentCustomer, _currency));
            _salePreparationSummaryFactory = new Lazy<SalePreparationSummaryFactory>(() => new SalePreparationSummaryFactory(_currency, _basketLineItemFactory.Value));
        }
    }
}