namespace Merchello.Bazaar.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    using Merchello.Bazaar.Models;
    using Merchello.Core;
    using Merchello.Core.Gateways;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Web;
    using Merchello.Web.Models.ContentEditing;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Web.Mvc;
    using Umbraco.Web.WebApi;

    /// <summary>
    /// The bazaar site API controller.
    /// </summary>
    [PluginController("Bazaar")]
    [JsonCamelCaseFormatter]
    public partial class BazaarSiteApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IMerchelloContext"/>
        /// </summary>
        private readonly IMerchelloContext _merchelloContext;

        /// <summary>
        /// The _store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// The <see cref="MerchelloHelper"/>.
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// The <see cref="ICurrency"/>.
        /// </summary>
        private ICurrency _currency;

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarSiteApiController"/> class.
        /// </summary>
        public BazaarSiteApiController()
            : this(MerchelloContext.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BazaarSiteApiController"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        public BazaarSiteApiController(IMerchelloContext merchelloContext)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            
            _merchelloContext = merchelloContext;
            
            _storeSettingService = _merchelloContext.Services.StoreSettingService;

            _merchello = new MerchelloHelper(_merchelloContext.Services);

            this.Initialize();
        }

        /// <summary>
        /// Returns a price for a product variant.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="optionChoiceKeys">
        /// The option choice keys.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation of the variant Price.
        /// </returns>
        [HttpGet]
        public object GetProductVariantPriceAndInventory(Guid productKey, string optionChoiceKeys)
        {
            var optionsArray = optionChoiceKeys.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => new Guid(x)).ToArray();

            // changed in 1.9.1 to use the MerchelloHelper for performance and to include the data modifiers
            var product = _merchello.Query.Product.GetByKey(productKey);
            var variant = product.GetProductVariantDisplayWithAttributes(optionsArray);

            var data = new
                           {
                               variant.OnSale,
                               SalePrice = ModelExtensions.FormatPrice(variant.SalePrice, _currency),
                               Price = ModelExtensions.FormatPrice(variant.Price, _currency),
                               TracksInventory = variant.TrackInventory,
                               variant.TotalInventoryCount
                           };

            return data;
        }

        /// <summary>
        /// The filter option choices.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <param name="productAttributeKey">
        /// The product attribute key
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ProductOptionDisplay}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<ProductOptionDisplay> FilterOptionChoices(Guid productKey, Guid productAttributeKey)
        {
            var product = _merchello.Query.Product.GetByKey(productKey);

            if (product == null)
            {
                var nullReference = new NullReferenceException("Product with key " + productKey + " returned null");
                LogHelper.Error<BazaarSiteApiController>("MerchelloHelper failed to retrieve product.", nullReference);
                throw nullReference;
            }

            // TODO move this to a service
            
            var returnOptions = new List<ProductOptionDisplay>();

            // this is the option the was just used in a selection
            var activeOption = product.ProductOptions.FirstOrDefault(po => po.Choices.Any(c => c.Key == productAttributeKey));

            if (activeOption == null) return returnOptions;

            ProductVariantDisplay[] variants; 

            // special case for a product with a single option
            // TODO clean this up
            if (1 == product.ProductOptions.Count())
            {
                variants = product.ProductVariants.Where(pv => pv.Available).ToArray();

                var addOption = new ProductOptionDisplay()
                                    {
                                        SortOrder = activeOption.SortOrder,
                                        Key = activeOption.Key,
                                        Name = activeOption.Name
                                    };
                var optionChoices = new List<ProductAttributeDisplay>();

                foreach (var choice in activeOption.Choices)
                {
                    if (ValidateOptionChoice(variants, choice.Key))
                    {
                        optionChoices.Add(new ProductAttributeDisplay()
                        {
                            Key = choice.Key,
                            Name = choice.Name,
                            Sku = choice.Sku,
                            OptionKey = choice.OptionKey,
                            SortOrder = choice.SortOrder
                        });
                    }
                }
                addOption.Choices = optionChoices;
                returnOptions.Add(addOption);
                return returnOptions;
            }

            returnOptions.Add(activeOption);
            

            var otherOptions = product.ProductOptions.Where(x => !x.Key.Equals(activeOption.Key)).ToArray();

            variants = product.ProductVariants.Where(pv => pv.Available && pv.Attributes.Any(att => att.Key == productAttributeKey)).ToArray();

            foreach (var option in otherOptions)
            {
                var addOption = new ProductOptionDisplay()
                                    {
                                        SortOrder = option.SortOrder,
                                        Key = option.Key,
                                        Name = option.Name
                                    };

                var optionChoices = new List<ProductAttributeDisplay>();

                foreach (var choice in option.Choices)
                {
                    if (ValidateOptionChoice(variants, choice.Key))
                    {
                        optionChoices.Add(new ProductAttributeDisplay()
                                            {
                                                Key = choice.Key,
                                                Name = choice.Name,
                                                Sku = choice.Sku,
                                                OptionKey = choice.OptionKey, 
                                                SortOrder = choice.SortOrder 
                                            });
                    }
                }

                addOption.Choices = optionChoices;

                returnOptions.Add(addOption);
            }

            return returnOptions;
        }

        /// <summary>
        /// Gets the collection of <see cref="IProvince"/> by country code.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProvince}"/>.
        /// </returns>
        [HttpGet]
        public IEnumerable<IProvince> GetProvincesForCountry(string countryCode)
        {
            return _storeSettingService.GetCountryByCode(countryCode).Provinces;
        }

        /// <summary>
        /// The update ship rate quote.
        /// </summary>
        /// <param name="customerToken">
        /// The customer token.
        /// </param>
        /// <param name="methodKey">
        /// The ship method key.
        /// </param>
        /// <returns>
        /// The <see cref="SalePreparationSummary"/>.
        /// </returns>
        [HttpGet]
        public UpdatedSaleSummary UpdateShipRateQuote(string customerToken, string methodKey)
        {
            var tokenKey = customerToken.DecryptWithMachineKey();
            var customerKey = new Guid(tokenKey);
            var customerBase = _merchelloContext.Services.CustomerService.GetAnyByKey(customerKey);

            var checkoutManager = customerBase.Basket().GetCheckoutManager();
            var shippingManager = checkoutManager.Shipping;
            var customerManager = checkoutManager.Customer;
            var paymentManager = checkoutManager.Payment;

            checkoutManager.Context.RaiseCustomerEvents = false;

            var shipment = customerBase.Basket().PackageBasket(customerManager.GetShipToAddress()).FirstOrDefault();
            var quote = shipment.ShipmentRateQuoteByShipMethod(methodKey);
            if (quote != null)
            {
                shippingManager.ClearShipmentRateQuotes();
                shippingManager.SaveShipmentRateQuote(quote);
            }

            var invoice = paymentManager.PrepareInvoice();

            var summary = new UpdatedSaleSummary
                              {
                                  TotalLabel = "Total",
                                  InvoiceTotal = ModelExtensions.FormatPrice(invoice.Total, _currency),
                                  TaxTotal = ModelExtensions.FormatPrice(invoice.TaxLineItems().Sum(x => x.TotalPrice), _currency),
                                  DiscountsTotal = ModelExtensions.FormatPrice(invoice.DiscountLineItems().Sum(x => x.TotalPrice), _currency),
                                  ShippingTotal = ModelExtensions.FormatPrice(invoice.ShippingLineItems().Sum(x => x.TotalPrice), _currency)
                              };
            return summary;
        }

        /// <summary>
        /// The get customer address.
        /// </summary>
        /// <param name="customerAddressKey">
        /// The customer address key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerAddressDisplay"/>.
        /// </returns>
        [HttpGet]
        public object GetCustomerAddress(Guid customerAddressKey)
        {
            var adr = _merchelloContext.Services.CustomerService.GetAddressByKey(customerAddressKey);
            return new 
                       { 
                           adr.Key,
                           AddressType = adr.AddressType.ToString().ToLowerInvariant(), 
                           adr.Label,
                           adr.FullName,
                           adr.Address1,
                           adr.Address2,
                           adr.Locality,
                           adr.Region,
                           adr.PostalCode,
                           adr.CountryCode,
                           adr.Company,
                           adr.Phone,
                           adr.IsDefault
                       };
        }

        /// <summary>
        /// Gets the payment method UI definition for the selected payment method.
        /// </summary>
        /// <param name="paymentMethodKey">
        /// The payment method key.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        [HttpGet]
        public PaymentMethodUiInfo GetPaymentMethodUi(Guid paymentMethodKey)
        {
            var paymentGatewayMethod = _merchelloContext.Gateways.Payment.GetPaymentGatewayMethodByKey(paymentMethodKey);

            var info = new PaymentMethodUiInfo() { PaymentMethodKey = paymentMethodKey };

            if (paymentGatewayMethod == null) return info;
            var type = paymentGatewayMethod.GetType();
            var att = type.GetCustomAttribute<GatewayMethodUiAttribute>(false);
            if (att == null) return info;

            info.Alias = att.Alias.Replace(".", string.Empty);

            return info;
        }

        /// <summary>
        /// The validate option choice.
        /// </summary>
        /// <param name="variants">
        /// The variants.
        /// </param>
        /// <param name="productAttributeKey">
        /// The product attribute key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool ValidateOptionChoice(IEnumerable<ProductVariantDisplay> variants, Guid productAttributeKey)
        {
            return variants.Any(pv => pv.Attributes.Any(pa => pa.Key.Equals(productAttributeKey)) && pv.Available);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        private void Initialize()
        {
            var setting = _storeSettingService.GetByKey(Core.Constants.StoreSettingKeys.CurrencyCodeKey);
            _currency = _storeSettingService.GetCurrencyByCode(setting.Value);
        }
    }
}