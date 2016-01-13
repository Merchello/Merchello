namespace Merchello.Bazaar.Factories
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Configuration;
    using System.Web.Mvc;

    using Merchello.Bazaar.Models;
    using Merchello.Bazaar.Models.Account;
    using Merchello.Bazaar.Models.ViewModels;
    using Merchello.Core;
    using Merchello.Core.Gateways.Payment;
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

    /// <summary>
    /// Represents a view model factory.
    /// </summary>
    public class ViewModelFactory : IViewModelFactory
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
                    AccountPageId = viewModel.Id,
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
                CheckoutPage = BazaarContentHelper.GetCheckoutPageContent(),
                ContinueShoppingPage = BazaarContentHelper.GetContinueShoppingContent(),
                ShowWishList = viewModel.ShowWishList && !_currentCustomer.IsAnonymous,
                WishListPageId = BazaarContentHelper.GetWishListContent().Id,
                BasketPageId = BazaarContentHelper.GetBasketContent().Id
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

            var checkoutManager = basket.GetCheckoutManager();

            viewModel.SaleSummary = _salePreparationSummaryFactory.Value.Build(checkoutManager);
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
        public CheckoutConfirmationModel CreateCheckoutConfirmation(RenderModel model, IBasket basket, IEnumerable<IShipmentRateQuote> shippingRateQuotes, IEnumerable<IPaymentGatewayMethod> paymentMethods, IEnumerable<PaymentMethodUiInfo> paymentMethodUiInfos)
        {
            var viewModel = this.Build<CheckoutConfirmationModel>(model);

            // Introduced resolvable forms in Bazaar 1.8.3
            try
            {
                viewModel.ResolvePaymentForms = bool.Parse(WebConfigurationManager.AppSettings["Bazaar:ResolvePaymentForms"]);
            }
            catch (Exception ex)
            {
                LogHelper.Error<ViewModelFactory>("Failed to find AppSetting 'Bazaar:ResolvePaymentForms", ex);
                viewModel.ResolvePaymentForms = false;
            }

            var paymentInfoArray = paymentMethodUiInfos as PaymentMethodUiInfo[] ?? paymentMethodUiInfos.ToArray();
            var paymentMethodsArray = paymentMethods as IPaymentGatewayMethod[] ?? paymentMethods.ToArray();

            var isAnonymous = basket.Customer.IsAnonymous;
            var allowedMethods = new List<IPaymentGatewayMethod>();

            // Payment methods, such as vaulted/stored credit cards may not be available to anonymous customers
            if (isAnonymous)
            {
                foreach (var method in paymentMethodsArray.ToArray())
                {
                    var addMethod = true;

                    var att = method.GetType().GetCustomAttribute<PaymentGatewayMethodAttribute>(false);
                    if (att != null && att.RequiresCustomer) addMethod = false;
                    if (addMethod) allowedMethods.Add(method);
                }
            }
            else
            {
                allowedMethods.AddRange(paymentMethodsArray);
            }

            var salesPreparation = basket.SalePreparation();

            // prepare the invoice
            var invoice = salesPreparation.PrepareInvoice();
            
            // get the existing shipMethodKey if any
            var shipmentLineItem = invoice.ShippingLineItems().FirstOrDefault();
            var shipMethodKey = shipmentLineItem != null ? shipmentLineItem.ExtendedData.GetShipMethodKey() : Guid.Empty;

            viewModel.CheckoutConfirmationForm = new CheckoutConfirmationForm()
            {
                ThemeName = viewModel.Theme,
                CustomerToken = basket.Customer.Key.ToString().EncryptWithMachineKey(),
                InvoiceSummary = new InvoiceSummary()
                                     {
                                         Invoice = invoice,
                                         Currency = viewModel.Currency,
                                         CurrentPageId = viewModel.Id
                                     },             
                ShipMethodKey   = shipMethodKey,
                ShippingQuotes = shippingRateQuotes.Select(x => new SelectListItem()
                                                                    {
                                                                        Value = x.ShipMethod.Key.ToString(),
                                                                        Text = string.Format("{0} ({1})", x.ShipMethod.Name, ModelExtensions.FormatPrice(x.Rate, _currency))
                                                                    }),
                PaymentMethods = (viewModel.ResolvePaymentForms
                     ? allowedMethods.Where(x => paymentInfoArray.Any(y => y.PaymentMethodKey == x.PaymentMethod.Key && y.UrlActionParams != null))
                     : paymentMethodsArray)
                     .Select(x => new SelectListItem() { Value = x.PaymentMethod.Key.ToString(), Text = x.PaymentMethod.Name }),

                PaymentMethodUiInfo = viewModel.ResolvePaymentForms ? 
                            paymentInfoArray.Where(x => x.UrlActionParams != null) :
                            paymentInfoArray,

                ReceiptPageId = viewModel.ReceiptPage.Id,

                ResolvePaymentForms = viewModel.ResolvePaymentForms
            };


            viewModel.RedeemCouponOfferForm = new RedeemCouponOfferForm()
                {
                    ThemeName = viewModel.Theme,
                    CurrencySymbol = viewModel.Currency.Symbol
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
        /// The create product collection.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCollectionModel"/>.
        /// </returns>
        public ProductCollectionModel CreateProductCollection(RenderModel model)
        {
            return this.Build<ProductCollectionModel>(model);
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
            viewModel.InvoiceSummary = new InvoiceSummary()
                                           {
                                               Invoice = invoice,
                                               Currency = viewModel.Currency
                                           };
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
                AccountPageId = BazaarContentHelper.GetAccountContent().Id
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
                WishListPageId = BazaarContentHelper.GetWishListContent().Id,
                BasketPageId = BazaarContentHelper.GetBasketContent().Id,
                ContinueShoppingPage = BazaarContentHelper.GetContinueShoppingContent()
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